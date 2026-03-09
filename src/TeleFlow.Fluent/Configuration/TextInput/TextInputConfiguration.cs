using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.TextInput;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput.Render;

namespace TeleFlow.Fluent.Configuration.TextInput;

public class TextInputConfiguration
{
    internal static TextInputConfiguration CreateDefault(Func<CommandStepCommitContext, string, Task> onCommit)
    {
        var renderOptions = new TextInputRenderServiceOptions
        {
            UserPrompt = TextInputDefaults.UserPrompt,
            ParseMode = TextInputDefaults.ParseMode
        };

        var stepOptions = new TextInputStepOptions
        {
            OnCommit = onCommit,
            NoMessageInputMessage = TextInputDefaults.NoMessageInputMessage
        };

        return new TextInputStepBuilder(stepOptions, renderOptions);
    }
    
    private TextInputStepOptions _options;
    private readonly TextInputRenderServiceOptions _renderOptions;

    public TextInputConfiguration UseMessageNoInput(string? message)
    {
        _options = _options with { NoMessageInputMessage = message };
        return this;
    }
    
    private TextInputConfiguration(TextInputStepOptions options, TextInputRenderServiceOptions renderOptions)
    {
        _options = options;
        _renderOptions = renderOptions;
    }
}