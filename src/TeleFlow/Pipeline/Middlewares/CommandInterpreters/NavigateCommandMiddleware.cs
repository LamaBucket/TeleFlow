using Microsoft.AspNetCore.Session;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands;
using TeleFlow.Commands.Factories;
using TeleFlow.Commands.Results;
using TeleFlow.Pipeline;
using TeleFlow.Pipeline.Contexts;

namespace TeleFlow.Pipeline.Middlewares.CommandInterpreters;

public class NavigateCommandMiddleware : CommandInterpreterBase<NavigateCommandResult>
{
    private readonly IHandler<CommandResultContext> _navigatedCommandInterpreter;

    private readonly ICommandFactory<ICommandHandler, NavigateCommandResult> _navigatorCommandFactory;

    private readonly IChatSessionStore _sessionStore;


    protected override bool ContinueAfterMatch => false;


    protected override async Task Handle(CommandResultContext<NavigateCommandResult> args)
    {
        await ExitOldCommand(args);

        var navigatedCommand = CreateNavigatedCommand(args.CommandResult);

        SessionContext newSession = CreateNewSessionContext(args.CommandResult, args);

        var commandResult = await navigatedCommand.Handle(args);

        await HandleNavigatedCommandResult(commandResult, newSession);
    }

    private async Task ExitOldCommand(CommandResultContext<NavigateCommandResult> context)
    {
        var chatId = context.GetService<IChatIdProvider>().GetChatId();

        await _sessionStore.RemoveAsync(chatId);
    }

    private ICommandHandler CreateNavigatedCommand(NavigateCommandResult commandResult)
    {
        var navigatedCommand = _navigatorCommandFactory.Create(commandResult);

        return navigatedCommand;
    }

    private SessionContext CreateNewSessionContext(NavigateCommandResult navigateCommandResult, UpdateContext updateContext)
    {
        ChatSession session = new(navigateCommandResult.CommandToNavigate);

        return new(session, updateContext);
    }

    private async Task HandleNavigatedCommandResult(CommandResult navigatedCommandResult, SessionContext args)
    {
        CommandResultContext navigatedContext = new(navigatedCommandResult, args);

        await _navigatedCommandInterpreter.Handle(navigatedContext);
    }


    public NavigateCommandMiddleware(IHandler<CommandResultContext> nextInMiddleware,
                                     IHandler<CommandResultContext> navigatedCommandInterpreter,
                                     ICommandFactory<ICommandHandler, NavigateCommandResult> navigatorCommandFactory,
                                     IChatSessionStore sessionStore) : base(nextInMiddleware)
    {
        _navigatedCommandInterpreter = navigatedCommandInterpreter;
        _navigatorCommandFactory = navigatorCommandFactory;
        _sessionStore = sessionStore;
    }
}