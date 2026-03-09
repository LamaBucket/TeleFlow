using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;

public sealed class ContactInputRenderServiceOptions
{
    public required ParseMode ParseMode { get; init; }

    public required Func<IServiceProvider, string> PromptText { get; init; }
    
    public Func<IServiceProvider, Contact, string>? AfterInputText { get; init; }

}