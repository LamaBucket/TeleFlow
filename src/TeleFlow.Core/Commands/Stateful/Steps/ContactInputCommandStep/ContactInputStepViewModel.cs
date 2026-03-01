using TeleFlow.Abstractions.State.Step;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInputCommandStep;

public record ContactInputStepViewModel(Contact? ContactShared) : StepViewModel
{
    public static ContactInputStepViewModel Default
        => new(ContactShared: null);
}