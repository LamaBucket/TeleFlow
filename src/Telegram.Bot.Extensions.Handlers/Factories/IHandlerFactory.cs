namespace LisBot.Common.Telegram.Factories;

public interface IHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    THandler Create();
}