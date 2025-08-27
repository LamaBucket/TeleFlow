using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Services;

public interface ICommandNameProvider
{
    string? GetCommandName(Update args);
}