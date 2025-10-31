using Telegram.Bot.Types;

namespace TeleFlow.Factories.CommandFactories;

public interface IHandlerFactoryWithArgs<THandler, TArgs, TFactoryArgs> : IHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    void SetContext(TFactoryArgs args);
}

public interface IHandlerFactoryWithArgs : IHandlerFactoryWithArgs<ICommandHandler, Update, Update>
{
}