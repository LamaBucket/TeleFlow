using TeleFlow.Abstractions.Engine.Commands;
using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.State.Chat;
using TeleFlow.Core.Commands.Decorators;
using TeleFlow.Core.Commands.Factories;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateless;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.CommandFactories;
using TeleFlow.Extensions.DI.CommandFactories.Routers;

namespace TeleFlow.Extensions.DI.Builders.Commands;

public class CommandRouterBuilder
{
    private readonly Dictionary<string, CommandDescriptor> _descriptors;

    private bool _built;


    public CommandFilterBuilder AddOrReplace(string name, ICommandFactory<ICommandHandler, ChatSession> factory)
    {
        EnsureNotBuilt();

        CommandDescriptor descriptor = new(name, factory);

        if (!_descriptors.TryAdd(name, descriptor))
            _descriptors[name] = descriptor;

        return new CommandFilterBuilder(this, descriptor, EnsureNotBuilt);
    }

    private void EnsureNotBuilt()
    {
        if(_built)
            throw new InvalidOperationException(
            $"{nameof(CommandRouterBuilder)} configuration is already finalized. " +
            $"You cannot call '{nameof(AddOrReplace)}' after the builder has been built.");
    }


    public CommandFilterBuilder AddOrReplace(string name, Func<ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));


    public CommandFilterBuilder AddOrReplace(string name, Func<ChatSession, ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));

    public CommandFilterBuilder AddSendText(string name, string message) 
        => AddOrReplace(name, (sp) => new SendMessageCommand(message));

    public CommandFilterBuilder AddMultiStep(string name, Action<CommandStepRouterBuilder> multiStepFactoryConfig, Func<IServiceProvider, Task<CommandResult>>? onCompleted = null)
    {
        return AddOrReplace(name, (session) => { 
            var stepSnapshot = session.CreateSnapshot();
            CommandStepRouterBuilder builder = new();

            multiStepFactoryConfig.Invoke(builder);

            onCompleted ??= async (serviceProvider) => { return CommandResult.Exit; };

            return new StepOrchestratorCommand(stepSnapshot, builder.Build(), onCompleted);
        });
    }


    public (SessionCommandRouter Session, NavigatedCommandRouter Navigated) Build()
    {
        EnsureNotBuilt();

        if (_descriptors.Count == 0)
            throw new InvalidOperationException("You did not add any commands!");

        _built = true;

        var sessionFactories = new Dictionary<string, ICommandFactory<ICommandHandler, ChatSession>>();
        var navFactories = new Dictionary<string, ICommandFactory<ICommandHandler, NavigateCommandParameters>>();

        foreach (var (name, descriptor) in _descriptors)
        {
            var sessionFactory = CompileSessionFactory(descriptor);
            sessionFactories[name] = sessionFactory;

            if (descriptor.NavigationEnabled)
            {
                navFactories[name] = CompileNavFactory(descriptor, sessionFactory);
            }
        }

        return (new SessionCommandRouter(new(sessionFactories)),
                new NavigatedCommandRouter(new(navFactories)));
    }

    private static ICommandFactory<ICommandHandler, ChatSession> CompileSessionFactory(CommandDescriptor descriptor)
    {
        if (descriptor.Filters.Count == 0)
            return descriptor.CommandFactory;

        return new LambdaCommandFactory<ChatSession>(session =>
        {
            ICommandHandler handler = descriptor.CommandFactory.Create(session);

            foreach(var filterFactory in descriptor.Filters)
            {
                var filter = filterFactory.Invoke();

                handler = new FilterCommandDecorator(handler, filter);
            }

            return handler;
        });
    }

    private static ICommandFactory<ICommandHandler, NavigateCommandParameters> CompileNavFactory(
        CommandDescriptor descriptor,
        ICommandFactory<ICommandHandler, ChatSession> finalSessionFactory)
    {
        return new LambdaCommandFactory<NavigateCommandParameters>(navArgs =>
        {
            var session = new ChatSession(descriptor.Name);

            var handler = finalSessionFactory.Create(session);

            return new NavigateCommandDecorator(handler, navArgs, descriptor.NavParamHandler);
        });
    }

    public CommandRouterBuilder()
    {
        _descriptors = [];
        _built = false;
    }
}