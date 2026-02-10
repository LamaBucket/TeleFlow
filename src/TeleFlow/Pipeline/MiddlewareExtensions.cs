using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Pipeline.Middlewares;

public static class MiddlewareExtensions
{
    public static long GetChatId(this Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From Message"),
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id ?? throw new NullReferenceException("Unable To Retrieve Chat Id From CallbackQuery"),
            _ => throw new InvalidOperationException("This MessageType is not supported"),
        };
    }

    public static string? GetCommandName(this Update args)
    {
        switch(args.Type)
        {
            case UpdateType.Message:
                return args.Message?.Text;
        }
        throw new InvalidOperationException("Unknown update type");
    }
}