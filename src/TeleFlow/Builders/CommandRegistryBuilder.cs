using TeleFlow.Commands;
using TeleFlow.Commands.Statefull;
using TeleFlow.Factories;
using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using Telegram.Bot.Requests;

namespace TeleFlow.Builders;

public class CommandRegistryBuilder
{
    private readonly Dictionary<string, ICommandFactory<ICommandHandler, ChatSession>> _factories;

    private bool _built;


    public CommandRegistryBuilder Add(string name, ICommandFactory<ICommandHandler, ChatSession> factory)
    {
        if(_built)
            throw new Exception("Cannot Add Commands after successfull build!");
        
        if(_factories.ContainsKey(name))
            throw new Exception($"Command with name {name} already exists");

        _factories.Add(name, factory);

        return this;
    }


    public CommandRegistryBuilder Add(string name, Func<ICommandHandler> factory) => Add(name, new LambdaCommandFactory(factory));


    public CommandRegistryBuilder Add(string name, Func<ChatSession, ICommandHandler> factory) => Add(name, new LambdaCommandFactory(factory));

    public CommandRegistryBuilder AddMultiStep(string name, Action<StepCommandFactoryBuilder> multiStepFactoryConfig, Func<IServiceProvider, Task<CommandResult>>? onCompleted = null)
    {
        return Add(name, (session) => { 
            ChatSessionStepState stepState = new(session.CurrentCommandStep, session.IsStepInitialized);
            StepCommandFactoryBuilder builder = new();

            multiStepFactoryConfig.Invoke(builder);

            onCompleted ??= async (serviceProvider) => { return CommandResult.Exit; };

            return new MultiStepCommand(stepState, builder.Build(), onCompleted);
        });
    }

    public CommandRegistryFactory Build()
    {
        if(_factories.Count == 0)
            throw new Exception("You did not add any commands!");

        _built = true;

        return new(new(_factories));
    }

    public CommandRegistryBuilder()
    {
        _factories = new();
        _built = false;
    }
}