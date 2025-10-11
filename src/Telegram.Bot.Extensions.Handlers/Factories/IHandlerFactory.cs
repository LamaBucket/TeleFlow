namespace Telegram.Bot.Extensions.Handlers.Factories;

public interface IHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    THandler Create();
}