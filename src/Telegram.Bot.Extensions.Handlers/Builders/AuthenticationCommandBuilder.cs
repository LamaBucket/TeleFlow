using LisBot.Common.Telegram.Models;

namespace LisBot.Common.Telegram.Builders;

public class AuthenticationCommandBuilder
{
    internal Func<UpdateListenerCommandFactoryArgs, ICommandHandler> HandlerIfAuthenticated => _handlerIfAuthenticated ?? throw new ArgumentNullException(nameof(_handlerIfAuthenticated));

    internal Func<UpdateListenerCommandFactoryArgs, ICommandHandler> HandlerIfNotAuthenticated => _handlerIfNotAuthenticated ?? throw new ArgumentNullException(nameof(_handlerIfNotAuthenticated));


    private Func<UpdateListenerCommandFactoryArgs, ICommandHandler>? _handlerIfAuthenticated;

    private Func<UpdateListenerCommandFactoryArgs, ICommandHandler>? _handlerIfNotAuthenticated;


    public AuthenticationCommandBuilder WithLambdaIfAuthenticated(Func<UpdateListenerCommandFactoryArgs, ICommandHandler> factory)
    {
        _handlerIfAuthenticated = factory;

        return this;
    } 

    public AuthenticationCommandBuilder WithLambdaIfNotAuthenticated(Func<UpdateListenerCommandFactoryArgs, ICommandHandler> factory)
    {
        _handlerIfNotAuthenticated = factory;

        return this;
    } 

}