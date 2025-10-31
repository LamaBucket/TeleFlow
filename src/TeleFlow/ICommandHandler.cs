using Telegram.Bot.Types;

namespace TeleFlow;

public interface ICommandHandler : IHandler<Update>
{
    event Func<Task>? CommandFinished;

    Task OnCommandCreated();
}
