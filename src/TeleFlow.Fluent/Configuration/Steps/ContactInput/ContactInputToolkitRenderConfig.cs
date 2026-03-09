using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Fluent.Configuration.Steps.ContactInput;

public class ContactInputToolkitRenderConfig
{
    public Func<IServiceProvider, string> PromptText { get; set; } = ContactInputDefaults.PromptText;

    public Func<IServiceProvider, Contact, string>? AfterInputText { get; set; }

    public ParseMode ParseMode { get; set; } = ContactInputDefaults.ParseMode;
}