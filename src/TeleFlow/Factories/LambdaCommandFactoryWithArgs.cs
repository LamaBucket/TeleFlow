using Telegram.Bot.Types;

namespace TeleFlow.Factories;

public class LambdaCommandFactoryWithArgs<TArgs> : LambdaHandlerFactoryWithArgs<ICommandHandler, Update, TArgs> where TArgs: class
{
    public LambdaCommandFactoryWithArgs(Func<TArgs, ICommandHandler> factory) : base(factory)
    {
    }
}