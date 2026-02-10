using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TeleFlow.Commands;
using TeleFlow.Commands.Statefull;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using Telegram.Bot.Requests;

namespace TeleFlow.Builders;

public class CommandRegistryBuilder
{
    private readonly Dictionary<string, CommandDescriptor> _descriptors;

    private bool _built;


    public CommandOptionsBuilder AddOrReplace(string name, ICommandFactory<ICommandHandler, ChatSession> factory)
    {
        EnsureNotBuilt();

        CommandDescriptor descriptor = new(name, factory);

        if (!_descriptors.TryAdd(name, descriptor))
            _descriptors[name] = descriptor;

        return new CommandOptionsBuilder(descriptor, EnsureNotBuilt);
    }

    private void EnsureNotBuilt()
    {
        if(_built)
            throw new Exception("Cannot configure Command Registry after finishing configuration!");
    }


    public CommandOptionsBuilder AddOrReplace(string name, Func<ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));


    public CommandOptionsBuilder AddOrReplace(string name, Func<ChatSession, ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));

    public CommandOptionsBuilder AddMultiStep(string name, Action<StepCommandFactoryBuilder> multiStepFactoryConfig, Func<IServiceProvider, Task<CommandResult>>? onCompleted = null)
    {
        return AddOrReplace(name, (session) => { 
            var stepSnapshot = session.CreateSnapshot();
            StepCommandFactoryBuilder builder = new();

            multiStepFactoryConfig.Invoke(builder);

            onCompleted ??= async (serviceProvider) => { return CommandResult.Exit; };

            return new FlowCommand(stepSnapshot, builder.Build(), onCompleted);
        });
    }


    public (SessionCommandRegistry Session, NavigatedCommandRegistry Navigated) Build()
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

        return (new SessionCommandRegistry(new(sessionFactories)),
                new NavigatedCommandRegistry(new(navFactories)));
    }

    private static ICommandFactory<ICommandHandler, ChatSession> CompileSessionFactory(CommandDescriptor descriptor)
    {
        if (descriptor.Interceptors.Count == 0)
            return descriptor.CommandFactory;

        return new LambdaCommandFactory<ChatSession>(session =>
        {
            ICommandHandler handler = descriptor.CommandFactory.Create(session);

            foreach(var interceptorFactory in descriptor.Interceptors)
            {
                var interceptor = interceptorFactory.Invoke();

                handler = new InterceptCommandDecorator(handler, interceptor);
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

    public CommandRegistryBuilder()
    {
        _descriptors = [];
        _built = false;
    }
}