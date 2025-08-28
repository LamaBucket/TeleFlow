using LisBot.Common.Telegram.Models;

namespace LisBot.Common.Telegram.Builders;

public class AuthenticationCommandBuilder
{
    internal Func<UpdateListenerCommandExecutionArgs, ICommandHandler> HandlerIfAuthenticated => _handlerIfAuthenticated ?? throw new ArgumentNullException(nameof(_handlerIfAuthenticated));

    internal Func<UpdateListenerCommandExecutionArgs, ICommandHandler> HandlerIfNotAuthenticated => _handlerIfNotAuthenticated ?? throw new ArgumentNullException(nameof(_handlerIfNotAuthenticated));


    private Func<UpdateListenerCommandExecutionArgs, ICommandHandler>? _handlerIfAuthenticated;

    private Func<UpdateListenerCommandExecutionArgs, ICommandHandler>? _handlerIfNotAuthenticated;


    public AuthenticationCommandBuilder WithLambdaIfAuthenticated(Func<UpdateListenerCommandExecutionArgs, ICommandHandler> factory)
    {
        _handlerIfAuthenticated = factory;

        return this;
    } 

    public AuthenticationCommandBuilder WithLambdaIfNotAuthenticated(Func<UpdateListenerCommandExecutionArgs, ICommandHandler> factory)
    {
        _handlerIfNotAuthenticated = factory;

        return this;
    } 

}