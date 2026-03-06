using TeleFlow.Abstractions.State.Step;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput;

public record ContactInputStepData(Contact? ContactShared, int? ShareContactReplyButtonMessageId) : StepData
{
    public static ContactInputStepData Default
        => new(null, null);
}