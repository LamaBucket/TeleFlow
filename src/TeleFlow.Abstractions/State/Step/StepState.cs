namespace TeleFlow.Abstractions.State.Step;

public abstract record class StepData;

public sealed record StepState<TData>(int MessageId, TData ViewModel)
    where TData : StepData;