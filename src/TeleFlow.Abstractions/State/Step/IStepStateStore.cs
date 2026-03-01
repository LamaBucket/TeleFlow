namespace TeleFlow.Abstractions.State.Step;

public interface IStepStateStore
{
    Task SetState<TVm>(long chatId, StepState<TVm> state) where TVm : StepViewModel;

    Task<StepState<TVm>?> GetState<TVm>(long chatId) where TVm : StepViewModel;

    Task RemoveState(long chatId);
}