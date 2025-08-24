using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Services;

public interface IMessageService<TMessage>
{
    Task<Message> SendMessage(TMessage message);
}