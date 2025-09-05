using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Extensions.Handlers.Services.Markup;

public interface IReplyMarkupManager
{
    Task ClearReplyButtons();

    Task CreateReplyButtonMarkup(string message, ReplyKeyboardMarkup markup);
}