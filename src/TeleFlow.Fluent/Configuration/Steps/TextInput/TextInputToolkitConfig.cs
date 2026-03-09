using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput.Render;
using TeleFlow.Fluent.Configuration.Base;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.TextInput;

public class TextInputToolkitConfig : StatefulStepToolkitConfig
{
    public TextInputToolkitRenderConfig Render { get; init; } = new();

    public string? MessageInputIsNotMessage { get; set; } = TextInputDefaults.NoMessageInputMessage;

    public string? MessageNoTextProvided { get; set; } = TextInputDefaults.NoTextProvidedMessage;
    
    public TextInputStepOptions BuildOptions(IStepRenderService<TextInputStepData> renderService, Func<CommandStepCommitContext, string, Task> onCommit)  
        => new()
        {
            RenderConfig = BuildStatefulOptions(renderService),
            OnUserCommit = onCommit,
            NoMessageInputMessage = MessageInputIsNotMessage,
            NoTextProvidedMessage = MessageNoTextProvided
        };
    public TextInputRenderServiceOptions BuildRenderOptions()
        => new()
        {
            ParseMode      = Render.ParseMode,
            PromptText     = Render.PromptText,
            AfterInputText = Render.AfterInputText
        };
    

}

public class TextInputToolkitRenderConfig
{
    public Func<IServiceProvider, string> PromptText { get; set; } = TextInputDefaults.PromptText;

    public Func<IServiceProvider, string, string>? AfterInputText { get; set; } = TextInputDefaults.AfterInputText;

    public ParseMode ParseMode { get; set; } = TextInputDefaults.ParseMode;
}