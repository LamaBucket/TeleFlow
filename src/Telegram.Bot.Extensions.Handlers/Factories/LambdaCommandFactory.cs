using LisBot.Common.Telegram.Commands;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class LambdaCommandFactory : LambdaHandlerFactory<ICommandHandler, Update>
{

    public static LambdaCommandFactory Create(Func<ICommandHandler> factory)
    {
        return new LambdaCommandFactory(factory);
    }

    public LambdaCommandFactory(Func<ICommandHandler> factory) : base(factory)
    {
    }
}

public class LambdaCommandFactory<TArgs> : LambdaHandlerFactory<ICommandHandler, Update>
{
    public LambdaCommandFactory(Func<TArgs, ICommandHandler> factory, TArgs args) : base(() => { return factory.Invoke(args); })
    {
    }
}