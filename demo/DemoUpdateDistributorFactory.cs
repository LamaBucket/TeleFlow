using demo.Services;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo;

public class DemoUpdateDistributorFactory : UpdateDistributorFactory
{
    protected override void SetupUpdateListenerFactoryBuilder(UpdateListenerCommandFactoryBuilder builder)
    {
        builder.ConfigureUpdateListenerForDemo();
    }

    protected override void ConfigureBeforeUpdateListenerHandler(UpdateDistributorNextHandlerFactoryBuilder options)
    {
        options
        .WithInterceptor(["/start"])
        .WithExceptionHandler(async (ex, args) =>
        {
            await args.FromUpdateDistributorArgs.MessageServiceString.SendMessage($"There was an Exception. Message: {ex.Message}");
            await args.Navigator.Handle("/start");
        });

    }

     public DemoUpdateDistributorFactory(IMessageServiceFactory<Message> messageServiceFactory,
                                       IMessageServiceFactory<string> messageServiceStringFactory,
                                       IMessageServiceFactory<Tuple<string, KeyboardButton>> messageServiceWithReplyKeyboardFactory,
                                       IReplyMarkupManagerFactory replyMarkupManagerFactory,
                                       IAuthenticationServiceFactory authenticationServiceFactory) : base(messageServiceFactory, messageServiceStringFactory, messageServiceWithReplyKeyboardFactory, replyMarkupManagerFactory, authenticationServiceFactory)
    {
    }
}