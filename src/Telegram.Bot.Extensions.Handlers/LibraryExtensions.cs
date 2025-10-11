using Telegram.Bot.Extensions.Handlers.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers;

public static class LibraryExtensions
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
            case UpdateType.CallbackQuery:
                return GetCommandNameFromCallbackQuery(args.CallbackQuery ?? throw new ArgumentNullException($"The callback query in {nameof(args)} was not provided"));
        }
        throw new InvalidOperationException("Unknown update type");
    }

    private static string? GetCommandNameFromCallbackQuery(CallbackQuery query)
    {
        string queryData = query.Data ?? throw new ArgumentNullException($"The data in {nameof(query)} was not provided");

        var args = JsonConvert.DeserializeObject<UniversalCommandFactoryViewModel>(queryData);

        return args?.CommandToExecute;
    }
}