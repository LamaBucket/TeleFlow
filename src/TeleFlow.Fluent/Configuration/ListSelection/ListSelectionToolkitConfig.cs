using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Fluent.Configuration.Base;

namespace TeleFlow.Fluent.Configuration.ListSelection;

public class ListSelectionToolkitConfig<T> : CallbackStepToolkitConfig
{
    public ListSelectionToolkitRenderConfig<T> Render { get; init; } = new();

    public ListSelectionToolkitConstraintsConfig<T> Constraints { get; init; } = new();

    public string? IndexOutOfRangeMessage { get; set; } = ListSelectionDefaults.IndexOutOfRangeMessage;
    public string? LastPageMessage { get; init; } = ListSelectionDefaults.LastPageMessage;
    public string? FirstPageMessage { get; init; } = ListSelectionDefaults.FirstPageMessage;

    public ListSelectionStepDataConstraints BuildConstraints()
    {
       return new() 
        {
            PageRows = Constraints.PageRows,
            PageColumns = Constraints.PageCols
        };
    } 
    public ListSelectionRenderServiceOptions<T> BuildRenderOption(ListSelectionRenderType renderType)
    {
        return new()
        {
            RenderType = renderType,
            DisplayNameParser = Render.DisplayNameParser,
            UserPrompt = Render.UserPrompt,
            IsPaddingOn = Render.IsPaddingOn,
            PrevPageButtonText = Render.PrevPageButtonText,
            NextPageButtonText = Render.NextPageButtonText,
            MultiSelectFinishButtonText = Render.MultiSelectFinishButtonText,
            EmptyButtonText = Render.EmptyButtonText,
        };
    }
    public ListSelectionStepOptions<T> BuildStepOptions(IStepRenderService<ListSelectionStepData<T>> renderService, Func<IServiceProvider, Task<IEnumerable<T>>> valueProvider, ListSelectionMode mode)
    {
        return new()
        {
            BaseOptions = BuildCallbackOptions(renderService),
            ValueProvider = valueProvider,
            Mode = mode,
            IndexOutOfRangeMessage = IndexOutOfRangeMessage,
            LastPageMessage = LastPageMessage,
            FirstPageMessage = FirstPageMessage
        };
    }

}