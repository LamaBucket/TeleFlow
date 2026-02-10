using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Callbacks;
using TeleFlow.Abstractions.Interactivity;
using TeleFlow.Abstractions.Messaging;
using TeleFlow.Abstractions.Sessions;
using TeleFlow.Commands.Flow.Steps.Interactive.Options;
using TeleFlow.Pipeline.Contexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Commands.Flow.Steps.Interactive;

public abstract class InteractiveFlowStepBase<TVM> : IFlowStep
    where TVM : class
{
    private readonly InteractiveStepBaseOptions _options;

    public async Task<FlowStepResult> Handle(UpdateContext args)
    {
        if(args.Update.Type != UpdateType.CallbackQuery || args.Update.CallbackQuery is null)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.NoCallbackQueryMessage);

        var (data, callbackMessageId) = ParseCallbackQuery(args.Update.CallbackQuery);

        var sp = args.ServiceProvider;

        var encoder = sp.GetRequiredService<ICallbackCodec>();
        if (!encoder.TryDecodeAction(data, out CallbackAction action))
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);


        var state = await LoadStateOrNull(sp);
        if (state is null)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);

        if (callbackMessageId != state.MessageId)
            return FlowStepResult.HoldOn(FlowStepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);


        return await HandleAction(sp, state, action);
    }

    public async Task OnEnter(IServiceProvider sp)
    {
        var vm = await CreateDefaultViewModel(sp);

        var markupEncoder = sp.GetRequiredService<ICallbackCodec>();
        var messageService = sp.GetRequiredService<IMessageSender>();
        
        var sent = await messageService.SendMessage(new OutgoingMessage
        {
            Text = _options.UserPrompt,
            ReplyMarkup = RenderMarkup(markupEncoder, vm)
        });

        var state = new InteractiveState<TVM>(sent.MessageId, vm);
        await UpsertState(sp, state);
    }

    
    protected abstract Task<TVM> CreateDefaultViewModel(IServiceProvider sp);

    protected abstract InlineKeyboardMarkup RenderMarkup(ICallbackCodec markupEncoder, TVM vm);

    protected abstract Task<FlowStepResult> HandleAction(IServiceProvider sp, InteractiveState<TVM> state, CallbackAction action);


    protected async Task UpsertAndRerender(IServiceProvider sp, InteractiveState<TVM> state)
    {
        await UpsertState(sp, state);

        var markupEncoder = sp.GetRequiredService<ICallbackCodec>();
        var messageService = sp.GetRequiredService<IMessageEditor>();

        var markup = RenderMarkup(markupEncoder, state.ViewModel);
        await messageService.EditInlineKeyboard(state.MessageId, markup);
    }

    protected async Task FinalizeStep(IServiceProvider sp)
    {
        var state = await LoadStateOrNull(sp);

        if(state is not null)
        {
            var editor = sp.GetRequiredService<IMessageEditor>();
            await editor.EditInlineKeyboard(state.MessageId, null);

            await RemoveState(sp);
        }
    }


    private static async Task<InteractiveState<TVM>?> LoadStateOrNull(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IInteractiveStateStore>();

        return await store.GetState<TVM>(chatId);
    }

    private static async Task UpsertState(IServiceProvider sp, InteractiveState<TVM> state)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IInteractiveStateStore>();

        await store.SetState(chatId, state);
    }

    private static async Task RemoveState(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IInteractiveStateStore>();

        await store.RemoveState(chatId);
    }

    private static (string data, int messageId) ParseCallbackQuery(CallbackQuery query)
    {
        if(query.Data is null)
            throw new Exception("Fatal Error (PIZDEC)");

        if(query.Message is null)
            throw new Exception("TeleFlow does not support InlineMode");

        return (query.Data, query.Message.MessageId);
    }


    public InteractiveFlowStepBase(InteractiveStepBaseOptions options)
    {
        _options = options;
    }
}