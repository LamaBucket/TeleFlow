using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Abstractions.Messaging;

public interface IMessageEditor
{
    Task<Message> EditText(int messageId, string text, ParseMode parseMode = ParseMode.None);
    
    Task<Message> EditInlineKeyboard(int messageId, InlineKeyboardMarkup? markup);
    
    Task Delete(int messageId);
}