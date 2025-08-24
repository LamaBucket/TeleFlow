namespace LisBot.Common.Telegram;

public interface IHandler<T>
{
    Task Handle(T args);
}