using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;
using TeleFlow.Fluent.Builders.Commands.Stateful;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters;
using TeleFlow.Fluent.Configuration.Steps.FileInput;

namespace TeleFlow.Fluent.Extensions.Commands.Stateful.Steps;

public static class FileInputToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<FileInputStepData> AddFileInput(this StepRouterBuilder builder, string userPrompt, Func<CommandStepCommitContext, FileReference, Task> onCommit)
        => AddFileInput(builder, onCommit, (configure) =>
        {
            configure.Render.PromptText = _ => userPrompt;
        });

    public static StepWithRenderPipelineFilterBuilder<FileInputStepData> AddFileInput(this StepRouterBuilder builder, Func<CommandStepCommitContext, FileReference, Task> onCommit, Action<FileInputToolkitConfig>? configure = null)
    {
        FileInputToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var renderOptions = config.BuildRenderOptions();
        return builder.Add(baseRenderService: () => new FileInputRenderService(renderOptions), 
                        factory: (renderService) => new FileInputStep(config.BuildOptions(renderService, onCommit)));
    }  
}
