using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Commands;

public abstract class NavigationCommandBase : UpdateListener, ICommandHandler
{
    public event Func<Task>? CommandFinished;


    protected async Task FinalizeCommand()
    {
        if(CommandFinished is not null)
            await CommandFinished.Invoke();
    }

    public virtual async Task OnCommandCreated()
    {
        
    }

    protected NavigationCommandBase(ICommandFactory handlerFactory, ICommandFactory<ICommandHandler, Update, string> stringHandlerFactory, IChatIdProvider chatIdProvider) : base(handlerFactory, stringHandlerFactory, chatIdProvider)
    {
    }
}