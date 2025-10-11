using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Commands.MultiStep.StepCommands;

public class EnumValueSelectionStepCommand<TEnum> : ListValueSelectionStepCommand<Tuple<TEnum>>
    where TEnum : Enum
{
    public EnumValueSelectionStepCommand(IMessageService<Message> messageService,
                                         string onCommandCreatedMessage,
                                         Func<TEnum, Task> onHandleUserSelect,
                                         Func<TEnum, string?> displayNameFormatter,
                                         MessageBuilderOptions options,
                                         StepCommand? next,
                                         IUserInputValidator? userInputValidator = null) 
    : base(messageService,onCommandCreatedMessage,
           (selectionResult) => { return onHandleUserSelect.Invoke(selectionResult.Item1); },
           options,
           async () =>
           {
               var allEnumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

               var enumValuesWithDisplayName = allEnumValues.Where(e => displayNameFormatter.Invoke(e) is not null).ToList();

               return enumValuesWithDisplayName.Select(e => new Tuple<TEnum>(e));

           }, 
           (enumValue) => { return displayNameFormatter.Invoke(enumValue.Item1) ?? throw new InvalidOperationException("The Enum Value passed in a formatter should have a display name."); },
           next,
           userInputValidator)
    {
    }
}