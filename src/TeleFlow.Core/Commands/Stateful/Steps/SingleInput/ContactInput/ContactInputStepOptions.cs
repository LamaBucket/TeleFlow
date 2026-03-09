using TeleFlow.Core.Commands.Stateful.Steps.Base;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.ContactInput;


public class ContactInputStepOptions
{
    public required StatefulStepOptions<ContactInputStepData> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, Contact, Task> OnUserCommit { get; init; }


    public string? ShareContactButtonText { get; init; }

    public Func<string, Contact?>? TryParseContactFromText { get; init; }


    public string? NoMessageInputMessage { get; init; }
    public string? NoContactProvidedMessage { get; init; }
    public string? NoTextProvidedMessage { get; init; }
    public string? InvalidTextContactMessage { get; init; }
}