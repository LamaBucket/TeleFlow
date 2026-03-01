namespace TeleFlow.Abstractions.State.Step;

public abstract record class StepViewModel;

public sealed record StepState<TVm>(int MessageId, TVm ViewModel)
    where TVm : StepViewModel;