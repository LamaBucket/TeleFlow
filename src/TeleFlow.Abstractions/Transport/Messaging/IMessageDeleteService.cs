namespace TeleFlow.Abstractions.Transport.Messaging;

public interface IMessageDeleteService
{
    Task Delete(int messageId);
}