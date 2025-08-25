using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories.CommandFactories;

public abstract class HandlerFactoryWithArgsBase<TArgs> : IHandlerFactoryWithArgs<ICommandHandler, Update, TArgs> 
    where TArgs: class
{
    private TArgs? _args;

    public ICommandHandler Create()
    {
        if (_args is null)
            throw new Exception("Args was null!");

        var handler = Create(_args);

        _args = null;

        return handler;
    }

    protected abstract ICommandHandler Create(TArgs args);

    public void SetContext(TArgs args)
    {
        _args = args;
    }

    protected HandlerFactoryWithArgsBase()
    {
    }
}
