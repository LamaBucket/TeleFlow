using Telegram.Bot.Types.ReplyMarkups;

namespace LisBot.Common.Telegram.Services;

public interface IReplyMarkupManager
{
    Task ClearReplyButtons();
}