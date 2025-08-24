using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram;

public static class LibraryExtensions
{
    public static long GetChatId(this Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id ?? throw new Exception("Unable To Retrieve Chat Id From Message"),
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id ?? throw new Exception("Unable To Retrieve Chat Id From CallbackQuery"),
            _ => throw new Exception("This MessageType is not supported"),
        };
    }

    public static string? GetCommandName(this Update args)
    {
        switch(args.Type)
        {
            case UpdateType.Message:
                return args.Message?.Text;
            case UpdateType.CallbackQuery:
                return GetCommandNameFromCallbackQuery(args.CallbackQuery ?? throw new Exception("The callback query was not provided"));
        }
        throw new Exception("Unknown update type");
    }

    private static string? GetCommandNameFromCallbackQuery(CallbackQuery query)
    {
        string queryData = query.Data ?? throw new Exception("CallbackQuery contains no data!");

        var args = JsonConvert.DeserializeObject<UniversalCommandFactoryViewModel>(queryData);

        return args?.CommandToExecute;
    }
}