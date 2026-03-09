using TeleFlow.Abstractions.Transport.Files;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;

public sealed record FileInputRenderServiceOptions
{
    public required ParseMode ParseMode { get; init; }

    public required Func<IServiceProvider, string> PromptText { get; init; }
    
    public Func<IServiceProvider, FileReference, string>? AfterInputText {get; init;}

}