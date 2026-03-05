namespace TeleFlow.Core.Transport.Callbacks;

public abstract record CallbackAction
{
    public abstract record CommandAction : CallbackAction
    {
        public sealed record Execute(string CommandKey) : CommandAction;
    }

    public abstract record StepAction : CallbackAction
    {
        public sealed record Back : StepAction;
        public sealed record Finish : StepAction;
        public sealed record GoTo(int StepNumber) : StepAction;
    }

    public abstract record UiAction : CallbackAction
    {
        public sealed record Finish : UiAction;
        public sealed record GoToPage(int Page) : UiAction;
        
        
        public sealed record NextPage : UiAction;
        public sealed record PrevPage : UiAction;
        public sealed record SelectIndex(int Index) : UiAction;
        
        public sealed record NoOperation : UiAction;
    }
}