using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands.MultiStep;

public abstract class StepCommand : ICommandHandler
{
    public event Func<Task>? CommandFinished;

    protected internal StepCommand? Next => _next;

    private StepCommand? _next;


    public async Task Handle(Update args)
    {
        if (_next is not null)
            await _next.Handle(args);
        else
            await HandleCurrentStep(args);
    }

    protected abstract Task HandleCurrentStep(Update args);


    public abstract Task OnCommandCreated();


    protected async Task FinalizeCommand()
    {
        if (_next is not null)
            throw new NullReferenceException($"Cannot Finalize current command ({_next} command is not NULL)");

        if (CommandFinished is not null)
            await CommandFinished.Invoke();
    }


    private async Task OnNextCommandFinished()
    {
        if (_next is null)
            throw new NullReferenceException(nameof(_next));

        _next.CommandFinished -= OnNextCommandFinished;
        _next = null;

        await OnCommandCreated();
    }


    protected StepCommand(StepCommand? next)
    {
        if (next is not null)
        {
            _next = next;
            _next.CommandFinished += OnNextCommandFinished;
        }
    }
}
