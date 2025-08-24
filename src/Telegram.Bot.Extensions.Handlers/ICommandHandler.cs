using Telegram.Bot.Types;

namespace LisBot.Common.Telegram;

public interface ICommandHandler : IHandler<Update>
{
    event Func<Task>? CommandFinished;

    Task OnCommandCreated();
}
