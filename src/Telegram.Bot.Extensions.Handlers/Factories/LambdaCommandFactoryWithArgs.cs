using LisBot.Common.Telegram.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class LambdaCommandFactoryWithArgs<TArgs> : LambdaHandlerFactoryWithArgs<ICommandHandler, Update, TArgs> where TArgs: class
{
    public LambdaCommandFactoryWithArgs(Func<TArgs, ICommandHandler> factory) : base(factory)
    {
    }
}