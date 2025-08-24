using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories;

public class LambdaHandlerFactory<THandler, TArgs> : IHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    private readonly Func<THandler> _factory;

    public THandler Create()
    {
        return _factory();
    }

    public LambdaHandlerFactory(Func<THandler> factory)
    {
        _factory = factory;
    }
}

public class LambdaHandlerFactory<THandler, TArgs, TFactoryArgs> : LambdaHandlerFactory<THandler, TArgs> where THandler : IHandler<TArgs>
{
    public LambdaHandlerFactory(Func<TFactoryArgs, THandler> factory, TFactoryArgs args) : base(() => { return factory.Invoke(args); })
    {
    }
}