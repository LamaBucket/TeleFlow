namespace TeleFlow.Abstractions.Interactivity;

public sealed class InteractiveState<TVm> where TVm : class
{
    public int MessageId { get; init; }
    
    public TVm ViewModel { get; init; }


    public InteractiveState(int messageId, TVm viewModel)
    {
        MessageId = messageId;
        ViewModel = viewModel;
    }
}