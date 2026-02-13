namespace TeleFlow.Abstractions.State.Step;

public interface IStepStateStore
{
    Task SetState<TVm>(long chatId, StepState<TVm> state) where TVm : class;

    Task<StepState<TVm>?> GetState<TVm>(long chatId) where TVm : class;

    Task RemoveState(long chatId);
}