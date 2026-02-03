using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Services;

namespace TeleFlow.Models.MultiStep;

public class StepCommitContext
{
    public long ChatId { get; init; }

    public IServiceProvider ServiceProvider { get; init; }

    public T GetRequired<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    public StepCommitContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ChatId = ServiceProvider.GetRequiredService<IChatIdProvider>().GetChatId();
    }
}