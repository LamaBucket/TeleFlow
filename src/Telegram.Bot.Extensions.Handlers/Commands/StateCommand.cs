using Telegram.Bot.Extensions.Handlers.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands;

public class StateCommand<TState> : ICommandHandler
{
    public event Func<Task>? CommandFinished;


    protected ICommandHandler Next => _next ?? throw new NullReferenceException(nameof(_next));

    private ICommandHandler? _next;


    private readonly IHandler<TState> _resultHandler;

    private readonly State<TState> _commandState;


    public async Task Handle(Update args)
    {
        await Next.Handle(args);
    }

    public async Task OnCommandCreated()
    {
        await Next.OnCommandCreated();
    }


    private async Task OnNextFinished()
    {
        Next.CommandFinished -= OnNextFinished;
        _next = null;

        await _resultHandler.Handle(_commandState.CurrentValue);

        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }


    public StateCommand(ICommandHandler next, IHandler<TState> resultHandler, State<TState> commandState)
    {
        _next = next;
        _next.CommandFinished += OnNextFinished;

        _resultHandler = resultHandler;
        _commandState = commandState;
    }
}