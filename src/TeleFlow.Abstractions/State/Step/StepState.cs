namespace TeleFlow.Abstractions.State.Step;

public sealed class StepState<TVm> where TVm : class
{
    public int MessageId { get; init; }
    
    public TVm ViewModel { get; init; }


    public StepState(int messageId, TVm viewModel)
    {
        MessageId = messageId;
        ViewModel = viewModel;
    }
}