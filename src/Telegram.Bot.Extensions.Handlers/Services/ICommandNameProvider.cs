using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Services;

public interface ICommandNameProvider
{
    string? GetCommandName(Update args);
}