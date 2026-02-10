using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Decorators;
using TeleFlow.Commands.Factories;
using TeleFlow.Commands.Flow;
using TeleFlow.Commands.Flow.Steps.Configuration;
using TeleFlow.Commands.Resolvers;
using TeleFlow.Commands.Results;

namespace TeleFlow.Commands.Configuration;

public class CommandResolversBuilder
{
    private readonly Dictionary<string, CommandDescriptor> _descriptors;

    private bool _built;


    public CommandInterceptorsBuilder AddOrReplace(string name, ICommandFactory<ICommandHandler, ChatSession> factory)
    {
        EnsureNotBuilt();

        CommandDescriptor descriptor = new(name, factory);

        if (!_descriptors.TryAdd(name, descriptor))
            _descriptors[name] = descriptor;

        return new CommandInterceptorsBuilder(descriptor, EnsureNotBuilt);
    }

    private void EnsureNotBuilt()
    {
        if(_built)
            throw new Exception("Cannot configure Command Registry after finishing configuration!");
    }


    public CommandInterceptorsBuilder AddOrReplace(string name, Func<ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));


    public CommandInterceptorsBuilder AddOrReplace(string name, Func<ChatSession, ICommandHandler> factory) => AddOrReplace(name, new LambdaCommandFactory<ChatSession>(factory));

    public CommandInterceptorsBuilder AddMultiStep(string name, Action<FlowStepResolverBuilder> multiStepFactoryConfig, Func<IServiceProvider, Task<CommandResult>>? onCompleted = null)
    {
        return AddOrReplace(name, (session) => { 
            var stepSnapshot = session.CreateSnapshot();
            FlowStepResolverBuilder builder = new();

            multiStepFactoryConfig.Invoke(builder);

            onCompleted ??= async (serviceProvider) => { return CommandResult.Exit; };

            return new FlowCommand(stepSnapshot, builder.Build(), onCompleted);
        });
    }


    public (SessionCommandResolver Session, NavigatedCommandResolver Navigated) Build()
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

        return (new SessionCommandResolver(new(sessionFactories)),
                new NavigatedCommandResolver(new(navFactories)));
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

    public CommandResolversBuilder()
    {
        _descriptors = [];
        _built = false;
    }
}