using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Models.Callbacks;
using TeleFlow.Models.Contexts;
using TeleFlow.Models.Interactive;
using TeleFlow.Models.Messaging;
using TeleFlow.Models.MultiStep;
using TeleFlow.Services;
using TeleFlow.Services.Callbacks;
using TeleFlow.Services.Messaging;
using TeleFlow.Services.ViewModels;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Commands.Statefull.StepCommands.Interactive;

public abstract class InteractiveStepBase<TVM> : IStepCommand
    where TVM : class
{
    private readonly InteractiveStepBaseOptions _options;

    public async Task<StepResult> Handle(UpdateContext args)
    {
        if(args.Update.Type != UpdateType.CallbackQuery || args.Update.CallbackQuery is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.NoCallbackQueryMessage);

        var (data, callbackMessageId) = ParseCallbackQuery(args.Update.CallbackQuery);

        var sp = args.ServiceProvider;

        var encoder = sp.GetRequiredService<ICallbackEncoder>();
        if (!encoder.TryDecodeAction(data, out CallbackAction action))
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);


        var state = await LoadStateOrNull(sp);
        if (state is null)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);

        if (callbackMessageId != state.MessageId)
            return StepResult.HoldOn(StepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);


        return await HandleAction(sp, state, action);
    }

    public async Task OnEnter(IServiceProvider sp)
    {
        var vm = await CreateDefaultViewModel(sp);

        var markupEncoder = sp.GetRequiredService<ICallbackEncoder>();
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

    protected abstract InlineKeyboardMarkup RenderMarkup(ICallbackEncoder markupEncoder, TVM vm);

    protected abstract Task<StepResult> HandleAction(IServiceProvider sp, InteractiveState<TVM> state, CallbackAction action);


    protected async Task UpsertAndRerender(IServiceProvider sp, InteractiveState<TVM> state)
    {
        await UpsertState(sp, state);

        var markupEncoder = sp.GetRequiredService<ICallbackEncoder>();
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
        var store = sp.GetRequiredService<IChatInteractiveStateStore>();

        return await store.GetState<TVM>(chatId);
    }

    private static async Task UpsertState(IServiceProvider sp, InteractiveState<TVM> state)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IChatInteractiveStateStore>();

        await store.SetState(chatId, state);
    }

    private static async Task RemoveState(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IChatInteractiveStateStore>();

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


    public InteractiveStepBase(InteractiveStepBaseOptions options)
    {
        _options = options;
    }
}

public class InteractiveStepBaseOptions
{
    public required string UserPrompt { get; init; }


    public string NoCallbackQueryMessage { get; init; } = "Use the buttons linked to the previous message";

    public string UnknownCallbackQueryActionMessage { get; init; } = "The query contains an unknown action";

    public string CallbackQueryExpiredMessage { get; init; } = "This button's query is expired";
}