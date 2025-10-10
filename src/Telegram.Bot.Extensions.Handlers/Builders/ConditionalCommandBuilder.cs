using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class ConditionalCommandBuilder<TBuildArgs> where TBuildArgs : class
{
    private Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, Task<bool>>? _condition;

    private Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, IHandlerFactory<ICommandHandler, Update>>? _ifOkHandler;

    private Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, IHandlerFactory<ICommandHandler, Update>>? _ifNotOkHandler;


    public ConditionalCommandBuilder<TBuildArgs> WithCondition(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, Task<bool>> condition)
    {
        _condition = condition;

        return this;
    }


    public ConditionalCommandBuilder<TBuildArgs> WithLambdaIfTrue(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, ICommandHandler> factory)
    {
        _ifOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandExecutionArgs<TBuildArgs>>(factory, args);
        };

        return this;
    }


    public ConditionalCommandBuilder<TBuildArgs> WithLambdaIfFalse(Func<UpdateListenerCommandExecutionArgs<TBuildArgs>, ICommandHandler> factory)
    {
        _ifNotOkHandler = (args) => {
            return new LambdaCommandFactory<UpdateListenerCommandExecutionArgs<TBuildArgs>>(factory, args);
        };

        return this;
    }


    public ConditionalCommand Build(UpdateListenerCommandExecutionArgs<TBuildArgs> args)
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