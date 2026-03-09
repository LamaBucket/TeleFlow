using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.Base;
using Telegram.Bot.Types;

namespace TeleFlow.Core.Commands.Stateful.Steps.SingleInput.ContactInput;

public record ContactInputStepData(Contact? Input, int? ShareContactReplyButtonMessageId) : SingleInputStepData<Contact>(Input)
{
    
}