using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Core.Commands.Stateful.Steps.Base;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;

public abstract class CallbackStepToolkitConfig : StatefulStepToolkitConfig
{
    public string? MessageNoCallbackQuery { get; set; } = CallbackStepDefaults.NoCallbackQueryMessage;

    public string? MessageUnknownCallbackQueryAction { get; set; } = CallbackStepDefaults.UnknownCallbackQueryActionMessage;

    public string? MessageCallbackQueryExpired { get; set; } = CallbackStepDefaults.CallbackQueryExpiredMessage;

    protected CallbackStepOptions<T> BuildCallbackOptions<T>(IStepRenderService<T> renderService) where T : StepData
        => new()
        {
            RenderConfig = BuildStatefulOptions(renderService),
            NoCallbackQueryMessage = MessageNoCallbackQuery,
            UnknownCallbackQueryActionMessage = MessageUnknownCallbackQueryAction,
            CallbackQueryExpiredMessage = MessageCallbackQueryExpired
        };
}