using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Transport.Files;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput;
using TeleFlow.Core.Commands.Stateful.Steps.ConfirmInput.Render;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput;
using TeleFlow.Core.Commands.Stateful.Steps.FileInput.Render;
using TeleFlow.Fluent.Builders.Commands.Stateful;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters;
using TeleFlow.Fluent.Configuration.Steps.ConfirmInput;
using TeleFlow.Fluent.Configuration.Steps.FileInput;

namespace TeleFlow.Fluent.Extensions.Commands.Stateful.Steps;

public static class ConfirmInputToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<ConfirmInputStepData> AddConfirm(this StepRouterBuilder builder, Func<bool, string> buttonTexts, Func<CommandStepCommitContext, bool, Task<CommandStepResult>> onCommit)
    {
        return builder.AddConfirm(onCommit, configure => { configure.Render.ConfirmButtonsParser = buttonTexts; });
    }
    
    public static StepWithRenderPipelineFilterBuilder<ConfirmInputStepData> AddConfirm(this StepRouterBuilder builder, Func<CommandStepCommitContext, bool, Task<CommandStepResult>> onCommit, Action<ConfirmInputToolkitConfig>? configure = null)
    {
        ConfirmInputToolkitConfig config = new();

        configure?.Invoke(config);

        var renderOptions = config.BuildRenderOptions();
        return builder.Add(baseRenderService: () => new ConfirmInputRenderService(renderOptions), 
            factory: (renderService) => new ConfirmInputStep(config.BuildOptions(renderService, onCommit)));
    }  
}