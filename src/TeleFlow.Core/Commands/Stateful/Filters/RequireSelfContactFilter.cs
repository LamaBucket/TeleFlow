using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Filters;

public class RequireSelfContactFilter : ICommandStepFilter
{
    private readonly string _errorMessage;

    public async Task<CommandStepResult> ExecuteOnUpdate(UpdateContext context, StepHandleDelegate next)
    {
        var chatId = context.GetService<IChatIdProvider>().GetChatId();

        if(context.Update.Type != UpdateType.Message || context.Update.Message is null || context.Update.Message.Contact is null)
            return await next(context);

        var contact = context.Update.Message.Contact;

        if (contact.UserId.HasValue && contact.UserId.Value == chatId)
            return await next(context);
        else 
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _errorMessage);
    }

    public Task ExecuteOnEnter(IServiceProvider serviceProvider, StepEnterDelegate next)
        => next(serviceProvider);

    public RequireSelfContactFilter(string errorMessage)
    {
        _errorMessage = errorMessage;
    }
}