using TeleFlow.Models.Messaging;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Messaging;

public static class MessagingExtensions
{
    public static Task<Message> SendMessage(this IMessageSender messageService, string message)
    {
        OutgoingMessage msg = new(){ Text = message };
        
        return messageService.SendMessage(msg);
    } 
}