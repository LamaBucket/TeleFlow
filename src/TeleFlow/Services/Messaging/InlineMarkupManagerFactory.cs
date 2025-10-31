namespace TeleFlow.Services.Messaging;

public abstract class InlineMarkupManagerFactory
{
    private readonly Dictionary<long, InlineMarkupManager> _managers = new();
    
    public InlineMarkupManager Create(long chatId)
    {
        if (!_managers.ContainsKey(chatId))
        {
            var manager = CreateInlineMarkupManager(chatId);
            _managers[chatId] = manager;
        }
        
        return _managers[chatId];
    }

    protected abstract InlineMarkupManager CreateInlineMarkupManager(long chatId);
}