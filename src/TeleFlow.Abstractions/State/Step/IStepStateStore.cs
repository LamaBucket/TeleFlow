namespace TeleFlow.Abstractions.State.Step;

public interface IStepStateStore
{
    Task SetState<TVm>(long chatId, StepState<TVm> state) where TVm : StepData;

    Task<StepState<TVm>?> GetState<TVm>(long chatId) where TVm : StepData;

    Task RemoveState(long chatId);
}