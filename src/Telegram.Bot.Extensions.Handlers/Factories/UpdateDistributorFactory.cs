using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Factories;

public class UpdateDistributorFactory<TBuildArgs> : IHandlerFactory<UpdateDistributor, Update>
    where TBuildArgs : class
{
    protected UpdateDistributor? Instance { get; set; }


    private readonly IUpdateDistributorArgsBuilder<TBuildArgs> _updateDistributorArgsBuilder;


    public UpdateDistributor Create()
    {
        if (Instance is not null)
            return Instance;

        var updateDistributor = BuildUpdateDistributor();

        BuildFinished(updateDistributor);

        return updateDistributor;
    }

    private UpdateDistributor BuildUpdateDistributor()
    {
        var listenerFactory = BuildNextFactory();

        return new UpdateDistributor(listenerFactory);
    }

    protected IHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider> BuildNextFactory()
    {
        return new LambdaHandlerFactoryWithArgs<IHandler<Update>, Update, IChatIdProvider>((chatIdProvider) =>
        {
            var args = _updateDistributorArgsBuilder.Build(chatIdProvider);

            UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> nextHandlerBuilder = new(SetupUpdateListenerFactoryBuilder);

            ConfigureBeforeUpdateListenerHandler(nextHandlerBuilder);

            return nextHandlerBuilder.Build(args);
        });
    }

    protected virtual void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder<TBuildArgs> builder)
    {

    }

    protected virtual void ConfigureBeforeUpdateListenerHandler(UpdateDistributorNextHandlerFactoryBuilder<TBuildArgs> options)
    {
        
    }

    protected virtual void BuildFinished(UpdateDistributor buildResult)
    {
        Instance = buildResult;

        // if you need the update distributor not to be Singleton - clear the created instance here.
    }

    public UpdateDistributorFactory(IUpdateDistributorArgsBuilder<TBuildArgs> updateDistributorArgsBuilder)
    {
        _updateDistributorArgsBuilder = updateDistributorArgsBuilder;
    }
}