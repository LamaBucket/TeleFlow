
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;
using TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.ContactInput;

public class ContactInputToolkitConfig : StatefulStepToolkitConfig
{
    public ContactInputToolkitRenderConfig Render { get; init; } = new();

    public Func<string, Contact?>? TryParseContactFromText { get; set; }

    public string? ShareContactButtonText { get; set; }

    public string? MessageInputIsNotMessage { get; set; } = ContactInputDefaults.NoMessageInputMessage;
    public string? NoContactProvidedMessage { get; set; } = ContactInputDefaults.NoContactProvidedMessage;
    public string? NoTextProvidedMessage { get; set; } = ContactInputDefaults.NoTextProvidedMessage;
    public string? InvalidTextContactMessage { get; set; } = ContactInputDefaults.InvalidTextContactMessage;


    public ContactInputStepOptions BuildOptions(IStepRenderService<ContactInputStepData> renderService, Func<CommandStepCommitContext, Contact, Task> onCommit)  
        => new()
        {
            RenderConfig = BuildStatefulOptions(renderService),
            OnUserCommit = onCommit,
            ShareContactButtonText = ShareContactButtonText,
            TryParseContactFromText = TryParseContactFromText,
            NoMessageInputMessage = MessageInputIsNotMessage,
            NoContactProvidedMessage = NoContactProvidedMessage,
            NoTextProvidedMessage = NoTextProvidedMessage,
            InvalidTextContactMessage = InvalidTextContactMessage
        };

    public ContactInputRenderServiceOptions BuildRenderOptions()
        => new()
        {
            ParseMode      = Render.ParseMode,
            PromptText     = Render.PromptText,
            AfterInputText = Render.AfterInputText
        };
}

public class ContactInputToolkitRenderConfig
{
    public Func<IServiceProvider, string> PromptText { get; set; } = ContactInputDefaults.PromptText;

    public Func<IServiceProvider, Contact, string>? AfterInputText { get; set; }

    public ParseMode ParseMode { get; set; } = ContactInputDefaults.ParseMode;
}