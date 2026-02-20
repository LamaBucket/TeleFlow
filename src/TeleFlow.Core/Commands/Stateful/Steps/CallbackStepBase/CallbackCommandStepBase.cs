using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.CallbackStepBase;

public abstract class CallbackCommandStepBase<TVM> : ICommandStep
    where TVM : class
{
    private readonly CallbackCommandStepBaseOptions _options;

    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        if(args.Update.Type != UpdateType.CallbackQuery || args.Update.CallbackQuery is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.NoCallbackQueryMessage);

        var (data, callbackMessageId) = ParseCallbackQuery(args.Update.CallbackQuery);

        var sp = args.ServiceProvider;


        var encoder = sp.GetRequiredService<ICallbackCodec>();
        if (!encoder.TryDecodeAction(data, out CallbackToken token))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);
        
        var actionParser = sp.GetRequiredService<ICallbackActionParser>();
        if(!actionParser.TryParse(token, out CallbackAction action))
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.UnknownCallbackQueryActionMessage);


        var state = await LoadStateOrNull(sp);
        if (state is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);

        if (callbackMessageId != state.MessageId)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.CallbackQueryExpiredMessage);


        return await HandleAction(sp, state, action);
    }

    public async Task OnEnter(IServiceProvider sp)
    {
        var vm = await CreateDefaultViewModel(sp);

        var actionParser = sp.GetRequiredService<ICallbackActionParser>();
        var markupEncoder = sp.GetRequiredService<ICallbackCodec>();
        var messageService = sp.GetRequiredService<IMessageSender>();
        
        var markupButtonActionCodec = (CallbackAction action) =>
        {
            var token = actionParser.Parse(action);
            return markupEncoder.EncodeAction(token);
        };

        var sent = await messageService.SendMessage(new OutgoingMessage
        {
            Text = _options.UserPrompt,
            ReplyMarkup = RenderMarkup(markupButtonActionCodec, vm)
        });

        var state = new StepState<TVM>(sent.MessageId, vm);
        await UpsertState(sp, state);
    }

    
    protected abstract Task<TVM> CreateDefaultViewModel(IServiceProvider sp);

    protected abstract InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec, TVM vm);

    protected abstract Task<CommandStepResult> HandleAction(IServiceProvider sp, StepState<TVM> state, CallbackAction action);


    protected async Task UpsertAndRerender(IServiceProvider sp, StepState<TVM> state)
    {
        await UpsertState(sp, state);

        var actionParser = sp.GetRequiredService<ICallbackActionParser>();
        var markupEncoder = sp.GetRequiredService<ICallbackCodec>();
        var messageService = sp.GetRequiredService<IMessageEditor>();

        var markupButtonActionCodec = (CallbackAction action) =>
        {
            var token = actionParser.Parse(action);
            return markupEncoder.EncodeAction(token);
        };

        var markup = RenderMarkup(markupButtonActionCodec, state.ViewModel);
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


    private static async Task<StepState<TVM>?> LoadStateOrNull(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IStepStateStore>();

        return await store.GetState<TVM>(chatId);
    }

    public static async Task UpsertState(IServiceProvider sp, StepState<TVM> state)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IStepStateStore>();

        await store.SetState(chatId, state);
    }

    private static async Task RemoveState(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IStepStateStore>();

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


    public CallbackCommandStepBase(CallbackCommandStepBaseOptions options)
    {
        _options = options;
    }
}