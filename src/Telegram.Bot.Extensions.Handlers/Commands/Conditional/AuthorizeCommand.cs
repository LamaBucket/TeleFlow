using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram.Commands.Conditional;

public class AuthorizeCommand : TellConditionNotMetCommand
{
    public AuthorizeCommand(IChatIdProvider userIdProvider,
                            Func<long, Task<bool>> checkIfUserIdIsOkFunction,
                            ICommandHandler handlerIfMeetsCondition,
                            IMessageService<string> messageService,
                            string conditionNotMetMessage) : base(() => { return checkIfUserIdIsOkFunction(userIdProvider.GetChatId()); }, handlerIfMeetsCondition, messageService, conditionNotMetMessage)
    {
        
    }
}