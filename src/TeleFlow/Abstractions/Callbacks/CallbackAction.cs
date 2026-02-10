namespace TeleFlow.Models.Callbacks;

public abstract record CallbackAction
{
    public abstract record Command : CallbackAction
    {
        public sealed record Execute(string CommandKey) : Command;
    }

    public abstract record Step : CallbackAction
    {
        public sealed record Back : Step;
        public sealed record Finish : Step;
        public sealed record GoTo(string StepId) : Step;
    }

    public abstract record Ui : CallbackAction
    {
        public sealed record NextPage : Ui;
        public sealed record PrevPage : Ui;
        public sealed record ToggleIndex(int Index) : Ui;
    }
}