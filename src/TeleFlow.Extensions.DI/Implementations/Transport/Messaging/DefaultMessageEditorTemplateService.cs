using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageEditorTemplateService : IMessageEditorTemplateService
{
    private Func<InlineKeyboardMarkup?, InlineKeyboardMarkup?>? _markupTemplate;

    private Func<string, ParseMode, string>? _textTemplate;


    public InlineKeyboardMarkup? UseTemplateOnInlineKeyboardEdit(InlineKeyboardMarkup? markup)
    {
        if(_markupTemplate is null)
            return markup;

        return _markupTemplate(markup);
    }

    public string UseTemplateOnTextEdit(string text, ParseMode parseMode)
    {
        if(_textTemplate is null)
            return text;

        return _textTemplate(text, parseMode);
    }


    public void ApplyTemplateOnInlineKeyboardEdit(Func<InlineKeyboardMarkup?, InlineKeyboardMarkup?> template)
    {
        var currentMarkupTemplate = _markupTemplate;

        _markupTemplate = (markup) =>
        {
            var oldTemplatedMarkup = currentMarkupTemplate?.Invoke(markup);
            return template.Invoke(oldTemplatedMarkup);
        };
    }

    public void ApplyTemplateOnTextEdit(Func<string, ParseMode, string> template)
    {        
        if(_textTemplate is null)
        {
            _textTemplate = template;
            return;    
        }

        var currentTextTemplate = _textTemplate;

        _textTemplate = (text, parseMode) =>
        {
            var oldTemplatedText = currentTextTemplate.Invoke(text, parseMode); 
            
            return template.Invoke(oldTemplatedText, parseMode); 
        };
    }

    public void ClearTemplates()
    {
        _markupTemplate = null;
        _textTemplate = null;
    }
}