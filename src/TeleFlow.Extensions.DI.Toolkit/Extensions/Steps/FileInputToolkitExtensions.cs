using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using TeleFlow.Extensions.DI.Toolkit.Configuration.FileInput;

namespace TeleFlow.Extensions.DI.Toolkit.Extensions.Steps;

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
