using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Commands.Stateful.Steps.CommandStepBase;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.CallbackCommandStepBase;

public abstract class CallbackStepBase<TViewModel> : StepBase<TViewModel>
    where TViewModel : StepViewModel
{
    private readonly CallbackStepBaseOptions<TViewModel> _options;

    protected override async Task<CommandStepResult> Handle(UpdateContext context, StepState<TViewModel> state)
    {
        if(context.Update.Type != UpdateType.CallbackQuery || context.Update.CallbackQuery is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoCallbackQueryMessage);

        var (data, callbackMessageId) = ParseCallbackQuery(context.Update.CallbackQuery);

        var sp = context.ServiceProvider;

        var encoder = sp.GetRequiredService<ICallbackCodec>();
        if (!encoder.TryDecodeAction(data, out CallbackToken token))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);
        
        var actionParser = sp.GetRequiredService<ICallbackActionParser>();
        if(!actionParser.TryParse(token, out CallbackAction action))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);

        if (callbackMessageId != state.MessageId)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);


        return await HandleAction(sp, state, action);
    }    

    private static (string data, int messageId) ParseCallbackQuery(CallbackQuery query)
    {
        if(query.Data is null)
            throw new InvalidOperationException("CallbackQuery.Data is null. TeleFlow expects callback queries produced by inline keyboards with non-empty callback data.");

        if(query.Message is null)
            throw new NotSupportedException("CallbackQuery.Message is null. TeleFlow requires callback queries bound to a chat message (inline mode is not supported).");

        return (query.Data, query.Message.MessageId);
    }

    protected abstract Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<TViewModel> state, CallbackAction action);


    public CallbackStepBase(CallbackStepBaseOptions<TViewModel> options) : base(options.RenderConfig)
    {
        _options = options;
    }
}