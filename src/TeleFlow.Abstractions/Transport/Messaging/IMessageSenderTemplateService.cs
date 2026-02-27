namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageSenderTemplateService
{
    OutgoingMessage UseTemplate(OutgoingMessage message);

    void ApplyTemplate(Func<OutgoingMessage, OutgoingMessage> template);

    void ClearTemplates();
}