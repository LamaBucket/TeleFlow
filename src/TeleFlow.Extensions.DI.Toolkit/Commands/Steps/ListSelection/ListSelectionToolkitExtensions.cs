using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.ListSelection;

public static class ListSelectionToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddMultiSelectEnum<T>(this StepRouterBuilder builder, Func<T, string?> enumValuesProvider, Func<CommandStepCommitContext, IEnumerable<T>, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null) where T : struct, Enum
    {
        Task<IEnumerable<T>> valueProvider(IServiceProvider _) =>
            Task.FromResult<IEnumerable<T>>(
                Enum.GetValues<T>()
                    .Where(v => !string.IsNullOrWhiteSpace(enumValuesProvider(v)))
                    .ToArray()
            );

        void configureWrapped(ListSelectionToolkitConfig<T> cfg)
        {
            configure?.Invoke(cfg);

            cfg.Render.DisplayNameParser = item => enumValuesProvider(item) ?? throw new Exception();
        }

        return AddMultiSelect(builder, valueProvider, onCommit, configureWrapped);
    }

    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddMultiSelect<T>(this StepRouterBuilder builder, IEnumerable<T> values, Func<CommandStepCommitContext, IEnumerable<T>, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null)
        => AddMultiSelect(builder, async _ => values, onCommit, configure);

    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddMultiSelect<T>(this StepRouterBuilder builder, Func<IServiceProvider, Task<IEnumerable<T>>> valueProvider, Func<CommandStepCommitContext, IEnumerable<T>, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null)
    {
        ListSelectionToolkitConfig<T> config = new();
        
        if(configure is not null)
            configure(config);

        var constraints = config.BuildConstraints();
        var renderOptions = config.BuildRenderOption(ListSelectionRenderType.MultiSelect);

        ListSelectionMode mode = new ListSelectionMode.MultiSelect<T>() { OnCommit = onCommit };

        return builder.Add(baseRenderService: () => new ListSelectionRenderService<T>(renderOptions, constraints), 
                           factory: (renderService) => new ListSelectionStep<T>(config.BuildStepOptions(renderService, valueProvider, mode), constraints));
    }


    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddSingleSelectEnum<T>(this StepRouterBuilder builder, Func<T, string?> enumValuesProvider, Func<CommandStepCommitContext, T, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null) where T : struct, Enum
    {
        Task<IEnumerable<T>> valueProvider(IServiceProvider _) =>
            Task.FromResult<IEnumerable<T>>(
                Enum.GetValues<T>()
                    .Where(v => !string.IsNullOrWhiteSpace(enumValuesProvider(v)))
                    .ToArray()
            );

        void configureWrapped(ListSelectionToolkitConfig<T> cfg)
        {
            configure?.Invoke(cfg);

            cfg.Render.DisplayNameParser = item => enumValuesProvider(item) ?? throw new Exception();
        }

        return AddSingleSelect(builder, valueProvider, onCommit, configureWrapped);
    }

    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddSingleSelect<T>(this StepRouterBuilder builder, IEnumerable<T> values, Func<CommandStepCommitContext, T, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null)
        => AddSingleSelect(builder, async _ => values, onCommit, configure);

    public static StepWithRenderPipelineFilterBuilder<ListSelectionStepData<T>> AddSingleSelect<T>(this StepRouterBuilder builder, Func<IServiceProvider, Task<IEnumerable<T>>> valueProvider, Func<CommandStepCommitContext, T, Task> onCommit, Action<ListSelectionToolkitConfig<T>>? configure = null)
    {
        ListSelectionToolkitConfig<T> config = new();
        
        if(configure is not null)
            configure(config);

        var constraints = config.BuildConstraints();
        var renderOptions = config.BuildRenderOption(ListSelectionRenderType.SingleSelect);

        ListSelectionMode mode = new ListSelectionMode.SingleSelect<T>() { OnCommit = onCommit };

        return builder.Add(baseRenderService: () => new ListSelectionRenderService<T>(renderOptions, constraints), 
                           factory: (renderService) => new ListSelectionStep<T>(config.BuildStepOptions(renderService, valueProvider, mode), constraints));
    }

}