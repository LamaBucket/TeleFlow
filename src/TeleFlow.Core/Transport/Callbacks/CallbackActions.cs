using static TeleFlow.Core.Transport.Callbacks.CallbackAction;

namespace TeleFlow.Core.Transport.Callbacks;

/// <summary>
/// Ergonomic facade for creating <see cref="CallbackAction"/> instances.
/// Keeps callback code readable (no long "new CallbackAction.Ui.*" chains) while preserving
/// strongly-typed actions for pattern matching/decoding. Contains no business logic.
/// </summary>
public static class CallbackActions
{
    public static class Ui
    {
        public static UiAction Noop => new UiAction.NoOperation();
        public static UiAction NextPage => new UiAction.NextPage();
        public static UiAction PrevPage => new UiAction.PrevPage();
        public static UiAction GoToPage(int page) => new UiAction.GoToPage(page);
        public static UiAction Select(int index) => new UiAction.SelectIndex(index);
    }

    public static class Step
    {
        public static StepAction Back => new StepAction.Back();
        public static StepAction Finish => new StepAction.Finish();
        public static StepAction GoTo(string stepId) => new StepAction.GoTo(stepId);
    }

    public static class Command
    {
        public static CommandAction Execute(string commandKey) => new CommandAction.Execute(commandKey);
    }
}