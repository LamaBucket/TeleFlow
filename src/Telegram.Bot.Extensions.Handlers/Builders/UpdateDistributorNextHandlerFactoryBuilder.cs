using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> where TBuildArgs : class
{
    private readonly Action<UpdateListenerCommandFactoryBuilder<TBuildArgs>> _updateListenerOptions;

    protected Queue<Func<IHandler<Update>, UpdateListenerCommandBuildArgs<TBuildArgs>, IHandler<Update>>> BeforeUpdateListenerCommandFactories => new(_beforeUpdateListenerCommandFactories);

    private readonly Queue<Func<IHandler<Update>, UpdateListenerCommandBuildArgs<TBuildArgs>, IHandler<Update>>> _beforeUpdateListenerCommandFactories;


    public IHandler<Update> Build(TBuildArgs args)
    {
        var updateListener = BuildUpdateListener(args);

        return BuildHandlersBeforeUpdateListener(new(args, updateListener), updateListener);
    }

    private UpdateListener BuildUpdateListener(TBuildArgs args)
    {
        var listenerFactory = BuildUpdateListenerFactory();

        listenerFactory.SetContext(args);
        return listenerFactory.Create();
    }

    private UpdateListenerFactory<TBuildArgs> BuildUpdateListenerFactory()
    {
        var listenerFactory = new UpdateListenerFactory<TBuildArgs>((updateDistributorArgs, navHandler) =>
        {
            var updateListenerFactoryBuilder = new UpdateListenerCommandFactoryBuilder<TBuildArgs>();

            _updateListenerOptions.Invoke(updateListenerFactoryBuilder);

            UpdateListenerCommandBuildArgs<TBuildArgs> args = new(updateDistributorArgs, navHandler);

            return updateListenerFactoryBuilder.Build(args);
        });

        return listenerFactory;
    }

    private IHandler<Update> BuildHandlersBeforeUpdateListener(UpdateListenerCommandBuildArgs<TBuildArgs> args, UpdateListener updateListener)
    {
        var beforeUpdateListenerCommandFactories = BeforeUpdateListenerCommandFactories;

        IHandler<Update> resultHandler = updateListener;

        while (beforeUpdateListenerCommandFactories.Count != 0)
        {
            var factory = beforeUpdateListenerCommandFactories.Dequeue();

            resultHandler = factory.Invoke(resultHandler, args);
        }

        return resultHandler;
    }


    public UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> WithExceptionHandler(Func<Exception, UpdateListenerCommandBuildArgs<TBuildArgs>, Task> handlerAction)
    {
        return WithCustomWrapUpdateListenerFunction((handler, args) =>
        {
            UpdateExceptionHandler exceptionHandler = new(handler, async (ex) =>
            {
                await handlerAction.Invoke(ex, args);
            });

            return exceptionHandler;
        });
    }

    public UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> WithInterceptor(string[] commandsToIntercept)
    {
        return WithCustomWrapUpdateListenerFunction<UpdateListener>((listener, args) =>
        {
            UpdateInterceptor interceptor = new(listener, commandsToIntercept);

            return interceptor;
        });
    }


    public UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> WithCustomWrapUpdateListenerFunction<THandler>(Func<THandler, UpdateListenerCommandBuildArgs<TBuildArgs>, IHandler<Update>> action)
        where THandler : IHandler<Update>
    {
        return WithCustomWrapUpdateListenerFunction((handler, args) =>
        {
            if (handler is THandler typeSafeHandler)
            {
                return action.Invoke(typeSafeHandler, args);
            }

            throw new InvalidCastException($"the {nameof(handler)} has a wrong type.");
        });
    } 

    public UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> WithCustomWrapUpdateListenerFunction(Func<IHandler<Update>, UpdateListenerCommandBuildArgs<TBuildArgs>, IHandler<Update>> action)
    {
        _beforeUpdateListenerCommandFactories.Enqueue(action);
        
        return this;
    }


    internal UpdateDistributorNextHandlerFactoryBuilder(Action<UpdateListenerCommandFactoryBuilder<TBuildArgs>> updateListenerOptions)
    {
        _updateListenerOptions = updateListenerOptions;
        _beforeUpdateListenerCommandFactories = [];
    }
}