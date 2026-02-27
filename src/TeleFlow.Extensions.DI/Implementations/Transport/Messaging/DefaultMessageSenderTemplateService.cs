using TeleFlow.Abstractions.Transport.Messaging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Messaging;

public class DefaultMessageSenderTemplateService : IMessageSenderTemplateService
{
    private Func<OutgoingMessage, OutgoingMessage>? _template;

    public OutgoingMessage UseTemplate(OutgoingMessage message)
    {
        if(_template is null)
            return message;

        return _template(message);
    }

    public void ApplyTemplate(Func<OutgoingMessage, OutgoingMessage> template)
    {
        if(_template is null)
        {
            _template = template;   
            return;
        }

        var oldTemplate = _template;

        _template = (message) =>
        {
            var messageWithOldTemplate = oldTemplate.Invoke(message);
            return template.Invoke(messageWithOldTemplate);
        };
    }

    public void ClearTemplates()
    {
        _template = null;
    }
}