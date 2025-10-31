using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Services.Markup;

public interface IReplyMarkupManager
{
    Task ClearReplyButtons();

    Task CreateReplyButtonMarkup(string message, ReplyKeyboardMarkup markup);
}