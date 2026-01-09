using TeleFlow.Models;
using TeleFlow.Models.CommandResults;
using TeleFlow.Models.Contexts;
using TeleFlow.Services;
using TeleFlow.Services.Registries;

namespace TeleFlow.Middlewares.CommandInterpreters.MultiStep;

public class StepInitMultiStepMiddleware : CommandInterpreterBase<GoToMultiStepResult>
{
    private readonly IChatSessionStore _sessionStore;

    private readonly IMultiStepCommandRegistry _commandRegistry;
    
    protected override bool ContinueAfterMatch => false;

    protected override async Task Handle(CommandResultContext<GoToMultiStepResult> args)
    {
        var session = args.Session;

        if (!session.IsStepInitialized)
        {
            await InitializeStep(args);

            session.InitializeStep();
            
            await UpdateSessionStore(args);
        }
    }

    private async Task InitializeStep(SessionContext context)
    {
        var commandName = context.Session.CurrentCommand;
        var commandStep = context.Session.CurrentCommandStep;

        var stepCommandFactory = _commandRegistry.GetFactoryFor(commandName);

        var stepCommand = stepCommandFactory.Create(commandStep);

        await stepCommand.OnEnter();
    }

    private async Task UpdateSessionStore(SessionContext args)
    {
        var session = args.Session;
        
        var chatId = args.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.SetAsync(chatId, session);
    }

    public StepInitMultiStepMiddleware(IHandler<CommandResultContext> next, IChatSessionStore sessionStore, IMultiStepCommandRegistry commandRegistry) : base(next)
    {
        _sessionStore = sessionStore;
        _commandRegistry = commandRegistry;
    }
}