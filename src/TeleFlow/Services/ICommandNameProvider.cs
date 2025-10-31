using Telegram.Bot.Types;

namespace TeleFlow.Services;

public interface ICommandNameProvider
{
    string? GetCommandName(Update args);
}