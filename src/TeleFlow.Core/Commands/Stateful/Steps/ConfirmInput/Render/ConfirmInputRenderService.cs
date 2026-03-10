using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;

public class ConfirmInputRenderService : IStepRenderService<ConfirmInputStepData>
{
    private readonly ConfirmInputRenderServiceOptions _options;

    public InlineMarkupMessage Render(IServiceProvider serviceProvider, ConfirmInputStepData data)
    {
        var callbackCodec = serviceProvider.GetRequiredService<ICallbackCodec>();
        var actionParser  = serviceProvider.GetRequiredService<ICallbackActionParser>();

        string MarkupButtonActionCodec(CallbackAction action)
        {
            return callbackCodec.EncodeAction(actionParser.Parse(action));
        }

        InlineMarkupMessage msg = new()
        {
            Text = _options.UserPrompt(serviceProvider, data.ValueSelected),
            ParseMode = _options.ParseMode,
            Markup = data.ValueSelected.HasValue ? null : RenderMarkup(MarkupButtonActionCodec)
        };

        return msg;
    }

    private InlineKeyboardMarkup RenderMarkup(Func<CallbackAction, string> markupButtonActionCodec)
    {
        InlineKeyboardBuilder b = new();

        var trueButtonText = _options.ConfirmButtonsParser(true);
        var falseButtonText = _options.ConfirmButtonsParser(false);

        b.ButtonCallback(trueButtonText, markupButtonActionCodec(CallbackActions.Ui.Select(1)));
        b.ButtonCallback(falseButtonText, markupButtonActionCodec(CallbackActions.Ui.Select(0)));

        return b.Build();
    }
    
    public ConfirmInputRenderService(ConfirmInputRenderServiceOptions options)
    {
        _options = options;
    }
}