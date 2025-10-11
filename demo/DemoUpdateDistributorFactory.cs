using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo;

public class DemoUpdateDistributorFactory : UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs>
{

    protected override void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder)
    {
        builder.ConfigureUpdateListenerForDemo();
    }

    protected override void ConfigureBeforeUpdateListenerHandler(UpdateDistributorNextHandlerFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> options)
    {
        options
        .WithInterceptor(["/start"])
        .WithExceptionHandler(async (ex, args) =>
        {
            await args.FromUpdateDistributorArgs.MessageServiceString.SendMessage($"There was an Exception. Message: {ex.Message}");
            await args.Navigator.Handle("/start");
        });

    }

    public DemoUpdateDistributorFactory(IUpdateDistributorArgsBuilder<UpdateDistributorNextHandlerBuildArgs> updateDistributorArgsBuilder) : base(updateDistributorArgsBuilder)
    {
    }
}