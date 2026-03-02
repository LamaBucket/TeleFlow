using TeleFlow.Abstractions.Transport.Files;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.StepRender.FileInputCommandStep;

public sealed class FileInputRenderServiceOptions
{
    public Func<IServiceProvider, string> PromptText { get; init; }
        = _ => "Please send us some file";

    public Func<IServiceProvider, FileReference, string>? AfterInputText { get; init; }
        = (_, input) =>
        {
            if(string.IsNullOrWhiteSpace(input.FileName))
                return "You sent a file.";
            else
                return $"You sent file {input.FileName}.";
        };

    public ParseMode ParseMode { get; init; } = ParseMode.Markdown;
}