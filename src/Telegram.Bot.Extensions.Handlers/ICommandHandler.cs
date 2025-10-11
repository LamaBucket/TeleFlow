using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers;

public interface ICommandHandler : IHandler<Update>
{
    event Func<Task>? CommandFinished;

    Task OnCommandCreated();
}
