using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput;

public static class ContactInputDefaults
{
    public static readonly string NoMessageInputMessage = "This Command accepts only messages";
    public static readonly string NoContactProvidedMessage = "Please share a contact.";
    public static readonly string NoTextProvidedMessage = "Please share a contact or provide contact text";
    public static readonly string InvalidTextContactMessage = "Please share a contact or provide a contact text in a valid format.";


    public static readonly ParseMode ParseMode = ParseMode.Markdown;
    public static readonly Func<IServiceProvider, string> PromptText = _ => "Please send a contact";
    public static readonly Func<IServiceProvider, Contact, string>? AfterInputText = (_, input) =>
        {
            if(string.IsNullOrWhiteSpace(input.FirstName))
                return "You shared a contact.";
            else
                if(string.IsNullOrWhiteSpace(input.LastName))
                    return $"You shared a contact of {input.FirstName}";
                else
                    return $"You shared a contact of {input.LastName} {input.FirstName}";
        };
}