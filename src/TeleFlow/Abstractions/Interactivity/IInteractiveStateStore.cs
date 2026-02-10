using TeleFlow.Models.Interactive;

namespace TeleFlow.Services.ViewModels;

public interface IInteractiveStateStore
{
    Task SetState<TVm>(long chatId, InteractiveState<TVm> state) where TVm : class;

    Task<InteractiveState<TVm>?> GetState<TVm>(long chatId) where TVm : class;

    Task RemoveState(long chatId);
}