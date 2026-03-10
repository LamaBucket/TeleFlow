using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput;
using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;
using TeleFlow.Fluent.Configuration.Steps.Base;
using Telegram.Bot.Types;

namespace TeleFlow.Fluent.Configuration.Steps.ConfirmInput;

public class ConfirmInputToolkitConfig : CallbackStepToolkitConfig
{
    public ConfirmInputToolkitRenderConfig Render { get; init; } = new();
    
    public string? InvalidButtonMessage { get; set; } = ConfirmInputDefaults.InvalidButtonMessage;
    
    
    public ConfirmInputStepOptions BuildOptions(IStepRenderService<ConfirmInputStepData> renderService, Func<CommandStepCommitContext, bool, Task<CommandStepResult>> onCommit)  
        => new()
        {
            CallbackStepOptions = BuildCallbackOptions(renderService),
            OnCommit = onCommit,
            InvalidButtonMessage = InvalidButtonMessage
        };

    public ConfirmInputRenderServiceOptions BuildRenderOptions()
        => new()
        {
            ParseMode      = Render.ParseMode,
            UserPrompt     = Render.UserPrompt,
            ConfirmButtonsParser = Render.ConfirmButtonsParser,
            Direction      = Render.Direction
        };
}