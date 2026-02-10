namespace TeleFlow.Commands.Factories;

public class LambdaCommandFactory<T> : ICommandFactory<ICommandHandler, T>
{
    private readonly Func<T, ICommandHandler> _factory;


    public ICommandHandler Create(T context) => _factory(context);


    public LambdaCommandFactory(Func<ICommandHandler> factory) : this((session) => factory())
    {
        
    }

    public LambdaCommandFactory(Func<T, ICommandHandler> factory)
    {
        _factory = factory;
    }
}