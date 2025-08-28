using LisBot.Common.Telegram.Factories.CommandFactories;

namespace LisBot.Common.Telegram.Factories;

public class LambdaHandlerFactoryWithArgs<THandler, TUpdate, TArgs> : IHandlerFactoryWithArgs<THandler, TUpdate, TArgs> where THandler: IHandler<TUpdate> where TArgs: class
{
    private TArgs? _args;

    private Func<TArgs, THandler> _factory;

    public THandler Create()
    {
        if(_args is null)
            throw new ArgumentNullException(nameof(_args));

        var handler = _factory.Invoke(_args);

        _args = null;

        return handler;
    }

    public void SetContext(TArgs args)
    {
        _args = args;
    }

    public LambdaHandlerFactoryWithArgs(Func<TArgs, THandler> factory)
    {
        _factory = factory;
    }
}