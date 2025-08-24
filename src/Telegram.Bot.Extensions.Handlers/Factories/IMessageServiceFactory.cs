using LisBot.Common.Telegram.Services;

namespace LisBot.Common.Telegram.Factories;

public interface IMessageServiceFactory<T>
{
    IMessageService<T> CreateMessageService(long chatId);
}