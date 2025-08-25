using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class LambdaCommandFactoryWithArgs<TArgs> : IHandlerFactoryWithArgs<ICommandHandler, Update, TArgs> where TArgs: class
{
    private TArgs? _args;

    private Func<TArgs, ICommandHandler> _factory;

    public ICommandHandler Create()
    {
        if(_args is null)
            throw new Exception("Args Were null");

        var handler = _factory.Invoke(_args);

        _args = null;

        return handler;
    }

    public void SetContext(TArgs args)
    {
        _args = args;
    }

    public LambdaCommandFactoryWithArgs(Func<TArgs, ICommandHandler> factory)
    {
        _factory = factory;
    }
}