using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Commands;

public interface ICommandHandler
{
    Task<CommandResult> Handle(UpdateContext update);
}