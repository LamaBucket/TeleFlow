using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using TeleFlow.Extensions.DI.Toolkit.Configuration.DateSelection;

namespace TeleFlow.Extensions.DI.Toolkit.Extensions.Steps;

public static class DateSelectionToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<DateSelectionStepData> AddYearSelect(this StepRouterBuilder builder, Func<CommandStepCommitContext, int, Task> onCommit, Action<DateSelectionToolkitConfig>? configure = null)
    {
        DateSelectionToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var constraints = config.BuildConstraints();
        var renderOptions = config.BuildRenderOptions();

        DateSelectionMode mode = new DateSelectionMode.YearOnly() { OnCommit = onCommit };

        return builder.Add(baseRenderService: () => new DateSelectionRenderService(constraints, renderOptions), 
                           factory: (renderService) => new DateSelectionStep(config.BuildStepOptions(renderService, mode), constraints));
    }

    public static StepWithRenderPipelineFilterBuilder<DateSelectionStepData> AddYearMonthSelect(this StepRouterBuilder builder, Func<CommandStepCommitContext, (int year, int month), Task> onCommit, Action<DateSelectionToolkitConfig>? configure = null)
    {
        DateSelectionToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var constraints = config.BuildConstraints();
        var renderOptions = config.BuildRenderOptions();

        DateSelectionMode mode = new DateSelectionMode.YearMonth() { OnCommit = onCommit };

        return builder.Add(baseRenderService: () => new DateSelectionRenderService(constraints, renderOptions), 
                           factory: (renderService) => new DateSelectionStep(config.BuildStepOptions(renderService, mode), constraints));
    }

    public static StepWithRenderPipelineFilterBuilder<DateSelectionStepData> AddDateSelect(this StepRouterBuilder builder, Func<CommandStepCommitContext, DateOnly, Task> onCommit, Action<DateSelectionToolkitConfig>? configure = null)
    {
        DateSelectionToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var constraints = config.BuildConstraints();
        var renderOptions = config.BuildRenderOptions();

        DateSelectionMode mode = new DateSelectionMode.YearMonthDay() { OnCommit = onCommit };

        return builder.Add(baseRenderService: () => new DateSelectionRenderService(constraints, renderOptions), 
                           factory: (renderService) => new DateSelectionStep(config.BuildStepOptions(renderService, mode), constraints));
    }
}