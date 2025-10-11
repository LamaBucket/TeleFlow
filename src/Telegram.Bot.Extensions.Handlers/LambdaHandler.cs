namespace Telegram.Bot.Extensions.Handlers;

public class LambdaHandler<T> : IHandler<T>
{
    private readonly Func<T, Task> _handler;

    public async Task Handle(T args)
    {
        await _handler.Invoke(args);
    }

    public LambdaHandler(Func<T, Task> handler)
    {
        _handler = handler;
    }
}
