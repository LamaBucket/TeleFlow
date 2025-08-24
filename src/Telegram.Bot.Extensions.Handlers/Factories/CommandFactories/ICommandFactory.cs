using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories.CommandFactories;

public interface ICommandFactory<THandler, TArgs, TFactoryArgs> : IHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    void SetContext(TFactoryArgs args);
}

public interface ICommandFactory : ICommandFactory<ICommandHandler, Update, Update>
{
}