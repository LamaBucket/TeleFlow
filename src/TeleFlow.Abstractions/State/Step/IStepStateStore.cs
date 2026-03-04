namespace TeleFlow.Abstractions.State.Step;

public interface IStepStateStore
{
    Task SetState<TData>(long chatId, StepState<TData> state) where TData : StepData;

    Task<StepState<TData>?> GetState<TData>(long chatId) where TData : StepData;

    Task RemoveState(long chatId);
}