using TeleFlow.Abstractions.Engine.Commands.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Engine.Commands;

public interface ICommandHandler
{
    Task<CommandResult> Handle(UpdateContext update);
}