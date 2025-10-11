namespace Telegram.Bot.Extensions.Handlers;

public interface IHandler<T>
{
    Task Handle(T args);
}