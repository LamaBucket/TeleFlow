using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageEditorTemplateService
{
    string UseTemplateOnTextEdit(string text, ParseMode parseMode);

    InlineKeyboardMarkup? UseTemplateOnInlineKeyboardEdit(InlineKeyboardMarkup? markup);


    void ApplyTemplateOnTextEdit(Func<string, ParseMode, string> template);

    void ApplyTemplateOnInlineKeyboardEdit(Func<InlineKeyboardMarkup?, InlineKeyboardMarkup?> template);


    void ClearTemplates();
}