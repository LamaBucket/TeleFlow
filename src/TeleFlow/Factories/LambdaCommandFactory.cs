using TeleFlow.Commands;
using TeleFlow.Models;

namespace TeleFlow.Factories;

public class LambdaCommandFactory : ICommandFactory<ICommandHandler, ChatSession>
{
    private readonly Func<ChatSession, ICommandHandler> _factory;


    public ICommandHandler Create(ChatSession context) => _factory(context);


    public LambdaCommandFactory(Func<ICommandHandler> factory) : this((session) => factory())
    {
        
    }

    public LambdaCommandFactory(Func<ChatSession, ICommandHandler> factory)
    {
        _factory = factory;
    }
}