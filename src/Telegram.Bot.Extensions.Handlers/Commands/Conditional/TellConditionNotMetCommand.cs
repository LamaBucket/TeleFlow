
using LisBot.Common.Telegram.Services;

namespace LisBot.Common.Telegram.Commands.Conditional;

public class TellConditionNotMetCommand : ConditionalCommand
{

    public TellConditionNotMetCommand(Func<Task<bool>> condition,
                                      ICommandHandler handlerIfMeetsCondition,
                                      IMessageService<string> messageService,
                                      string conditionNotMetMessage) : base(condition, handlerIfMeetsCondition, new SendTextCommand(messageService, conditionNotMetMessage))
    {
    }
}