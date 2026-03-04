using TeleFlow.Abstractions.State.Step;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput;

public record ContactInputStepData(Contact? ContactShared) : StepData
{
    public static ContactInputStepData Default
        => new(ContactShared: null);
}