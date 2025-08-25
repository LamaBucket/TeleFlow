using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class ConditionalCommandBuilder
{

    private Func<UpdateListenerCommandFactoryArgs, Task<bool>>? _condition;

    private Func<UpdateListenerCommandFactoryArgs, IHandlerFactory<ICommandHandler, Update>>? _ifOkHandler;

    private Func<UpdateListenerCommandFactoryArgs, IHandlerFactory<ICommandHandler, Update>>? _ifNotOkHandler;


    public ConditionalCommandBuilder WithCondition(Func<UpdateListenerCommandFactoryArgs, Task<bool>> condition)
    {
        _condition = condition;

        return this;
    }


    public ConditionalCommandBuilder WithLambdaIfTrue(Func<UpdateListenerCommandFactoryArgs, ICommandHandler> factory)
    {
        _ifOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandFactoryArgs>(factory, args);
        };

        return this;
    }


    public ConditionalCommandBuilder WithSendTextIfFalse(string message)
    {
        return WithLambdaIfFalse((args) => { 
            return new SendTextCommand(args.MessageServiceString, message);
            });
    }

    public ConditionalCommandBuilder WithLambdaIfFalse(Func<UpdateListenerCommandFactoryArgs, ICommandHandler> factory)
    {
        _ifNotOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandFactoryArgs>(factory, args);
        };

        return this;
    }


    public ConditionalCommand Build(UpdateListenerCommandFactoryArgs args)
    {
        if(_condition is null)
            throw new Exception("Condition Cannot Be Null!");

        if(_ifOkHandler is null)
            throw new Exception("Execution Cannot Be Null!");

        if(_ifNotOkHandler is null)
            throw new Exception("Execution Cannot Be Null!");

        return new(async () => { return await _condition.Invoke(args); }, _ifOkHandler.Invoke(args).Create(), _ifNotOkHandler.Invoke(args).Create());
    }

    internal ConditionalCommandBuilder()
    {
    }
}