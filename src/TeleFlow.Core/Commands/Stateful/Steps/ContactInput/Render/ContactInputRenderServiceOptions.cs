using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;

public sealed class ContactInputRenderServiceOptions
{
    public Func<IServiceProvider, string> PromptText { get; init; }
        = _ => "Please send us some file";

    public Func<IServiceProvider, Contact, string>? AfterInputText { get; init; }
        = (_, input) =>
        {
            if(string.IsNullOrWhiteSpace(input.FirstName))
                return "You shared a contact.";
            else
                if(string.IsNullOrWhiteSpace(input.LastName))
                    return $"You shared a contact of {input.FirstName}";
                else
                    return $"You shared a contact of {input.LastName} {input.FirstName}";
        };

    public ParseMode ParseMode { get; init; } = ParseMode.Markdown;
}