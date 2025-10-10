using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class ConditionalCommandBuilder
{
    private Func<UpdateListenerCommandExecutionArgs, Task<bool>>? _condition;

    private Func<UpdateListenerCommandExecutionArgs, IHandlerFactory<ICommandHandler, Update>>? _ifOkHandler;

    private Func<UpdateListenerCommandExecutionArgs, IHandlerFactory<ICommandHandler, Update>>? _ifNotOkHandler;


    public ConditionalCommandBuilder WithCondition(Func<UpdateListenerCommandExecutionArgs, Task<bool>> condition)
    {
        _condition = condition;

        return this;
    }


    public ConditionalCommandBuilder WithLambdaIfTrue(Func<UpdateListenerCommandExecutionArgs, ICommandHandler> factory)
    {
        _ifOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandExecutionArgs>(factory, args);
        };

        return this;
    }


    public ConditionalCommandBuilder WithSendTextIfFalse(string message)
    {
        return WithLambdaIfFalse((args) => { 
            return new SendTextCommand(args.MessageServiceString, message);
            });
    }

    public ConditionalCommandBuilder WithLambdaIfFalse(Func<UpdateListenerCommandExecutionArgs, ICommandHandler> factory)
    {
        _ifNotOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandExecutionArgs>(factory, args);
        };

        return this;
    }


    public ConditionalCommand Build(UpdateListenerCommandExecutionArgs args)
    {
        if(_condition is null)
            throw new ArgumentNullException(nameof(_condition));

        if(_ifOkHandler is null)
            throw new ArgumentNullException(nameof(_ifOkHandler));

        if(_ifNotOkHandler is null)
            throw new ArgumentNullException(nameof(_ifNotOkHandler));

        return new(async () => { return await _condition.Invoke(args); }, _ifOkHandler.Invoke(args).Create(), _ifNotOkHandler.Invoke(args).Create());
    }

    internal ConditionalCommandBuilder()
    {
    }
}