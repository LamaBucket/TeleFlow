using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput;
using TeleFlow.Core.Commands.Stateful.Steps.TextInput.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using TeleFlow.Extensions.DI.Toolkit.Configuration.TextInput;

namespace TeleFlow.Extensions.DI.Toolkit.Extensions.Steps;

public static class TextInputToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<TextInputStepData> AddTextInput(this StepRouterBuilder builder, string userPrompt, Func<CommandStepCommitContext, string, Task> onCommit)
        => AddTextInput(builder, onCommit, (configure) =>
        {
            configure.Render.PromptText = _ => userPrompt;
        });

    public static StepWithRenderPipelineFilterBuilder<TextInputStepData> AddTextInput(this StepRouterBuilder builder, Func<CommandStepCommitContext, string, Task> onCommit, Action<TextInputToolkitConfig>? configure = null)
    {
        TextInputToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var renderOptions = config.BuildRenderOptions();
        return builder.Add(baseRenderService: () => new TextInputRenderService(renderOptions), 
                           factory: (renderService) => new TextInputStep(config.BuildOptions(renderService, onCommit)));
    }
 
}
