using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Builders;

public class UpdateDistributorNextHandlerFactoryBuilder
{
    private readonly Action<UpdateListenerCommandFactoryBuilder> _updateListenerOptions;

    protected Queue<Func<IHandler<Update>, UpdateListenerCommandBuildArgs, IHandler<Update>>> BeforeUpdateListenerCommandFactories => new(_beforeUpdateListenerCommandFactories);

    private readonly Queue<Func<IHandler<Update>, UpdateListenerCommandBuildArgs, IHandler<Update>>> _beforeUpdateListenerCommandFactories;


    public IHandler<Update> Build(UpdateDistributorNextHandlerBuildArgs args)
    {
        var updateListener = BuildUpdateListener(args);

        return BuildHandlersBeforeUpdateListener(new(args, updateListener), updateListener);
    }

    private UpdateListener BuildUpdateListener(UpdateDistributorNextHandlerBuildArgs args)
    {
        var listenerFactory = BuildUpdateListenerFactory();

        listenerFactory.SetContext(args);
        return listenerFactory.Create();
    }

    private UpdateListenerFactory BuildUpdateListenerFactory()
    {
        var listenerFactory = new UpdateListenerFactory((updateDistributorArgs, navHandler) =>
        {
            var updateListenerFactoryBuilder = new UpdateListenerCommandFactoryBuilder();

            _updateListenerOptions.Invoke(updateListenerFactoryBuilder);

            UpdateListenerCommandBuildArgs args = new(updateDistributorArgs, navHandler);

            return updateListenerFactoryBuilder.Build(args);
        });

        return listenerFactory;
    }

    private IHandler<Update> BuildHandlersBeforeUpdateListener(UpdateListenerCommandBuildArgs args, UpdateListener updateListener)
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


    public UpdateDistributorNextHandlerFactoryBuilder WithExceptionHandler(Func<Exception, UpdateListenerCommandBuildArgs, Task> handlerAction)
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

    public UpdateDistributorNextHandlerFactoryBuilder WithInterceptor(string[] commandsToIntercept)
    {
        return WithCustomWrapUpdateListenerFunction<UpdateListener>((listener, args) =>
        {
            UpdateInterceptor interceptor = new(listener, commandsToIntercept);

            return interceptor;
        });
    }


    public UpdateDistributorNextHandlerFactoryBuilder WithCustomWrapUpdateListenerFunction<THandler>(Func<THandler, UpdateListenerCommandBuildArgs, IHandler<Update>> action)
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

    public UpdateDistributorNextHandlerFactoryBuilder WithCustomWrapUpdateListenerFunction(Func<IHandler<Update>, UpdateListenerCommandBuildArgs, IHandler<Update>> action)
    {
        _beforeUpdateListenerCommandFactories.Enqueue(action);
        
        return this;
    }


    internal UpdateDistributorNextHandlerFactoryBuilder(Action<UpdateListenerCommandFactoryBuilder> updateListenerOptions)
    {
        _updateListenerOptions = updateListenerOptions;
        _beforeUpdateListenerCommandFactories = [];
    }
}