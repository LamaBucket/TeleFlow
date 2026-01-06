using Telegram.Bot.Types;

namespace TeleFlow.Factories.CommandFactories;

public interface IHandlerFactoryWithContext<out THandler, TArgs, TContext> where THandler : IHandler<TArgs>
{
    THandler Create(TContext context);
}