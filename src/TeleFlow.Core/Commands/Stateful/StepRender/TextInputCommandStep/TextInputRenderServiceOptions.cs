using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.StepRender.TextInputCommandStep;

public sealed class TextInputRenderServiceOptions
{
    public Func<IServiceProvider, string> PromptText { get; init; }
        = _ => "Please enter some text";

    public Func<IServiceProvider, string, string>? AfterInputText { get; init; }
        = (_, input) => $"You entered: {input}";

    public ParseMode ParseMode { get; init; } = ParseMode.Markdown;
}