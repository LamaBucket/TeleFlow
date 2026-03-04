using TeleFlow.Core.Commands.Stateful.Steps.Base;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput;


public class ContactInputStepOptions
{
    public required StatefulStepOptions<ContactInputStepViewModel> RenderConfig { get; init; }

    public required Func<CommandStepCommitContext, Contact, Task> OnUserCommit { get; init; }


    public string? ShareContactButtonText { get; init; } = null;

    public Func<string, Contact?>? TryParseContactFromText { get; init; } = null;


    public string NoMessageInputMessage { get; init; } = "This Command accepts only messages";
    public string NoContactProvidedMessage { get; init; } = "Please share a contact.";
    public string NoTextProvidedMessage { get; init; } = "Please share a contact or provide contact text";
    public string InvalidTextContactMessage { get; init; } = "Please share a contact or provide a contact text in a valid format.";
}