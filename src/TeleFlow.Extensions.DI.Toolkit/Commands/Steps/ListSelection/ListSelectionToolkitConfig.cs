using System.Security.AccessControl;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection;
using TeleFlow.Core.Commands.Stateful.Steps.ListSelection.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using TeleFlow.Extensions.DI.Toolkit.Commands.Steps.BaseConfigs;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.ListSelection;

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
            MultiSelectFinishButtonText = Render.MultiSelectFinishButtonText
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

public class ListSelectionToolkitRenderConfig<T>
{
    public ListSelectionDisplayNameParser<T> DisplayNameParser { get; set; } = ListSelectionDefaults.DisplayNameParser;

    public ListSelectionUserPrompt<T> UserPrompt { get; set; } = ListSelectionDefaults.UserPrompt;

    public bool IsPaddingOn { get; set; } = ListSelectionDefaults.IsPaddingOn;

    public string PrevPageButtonText { get; set; } = ListSelectionDefaults.PrevPageButtonText;
    public string NextPageButtonText { get; set; } = ListSelectionDefaults.NextPageButtonText;
    public string MultiSelectFinishButtonText { get; set; } = ListSelectionDefaults.MultiSelectFinishButtonText;
}

public class ListSelectionToolkitConstraintsConfig<T>
{
    public int PageRows { get; set; } = ListSelectionDefaults.PageRows;

    public int PageCols { get; set; } = ListSelectionDefaults.PageCols;
}