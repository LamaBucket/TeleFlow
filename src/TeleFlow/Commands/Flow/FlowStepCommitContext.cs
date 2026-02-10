using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Sessions;

namespace TeleFlow.Commands.Flow;

//This context is provided in OnCommit() Funcs for user's convenience
public class FlowStepCommitContext
{
    public long ChatId { get; init; }

    public IServiceProvider ServiceProvider { get; init; }

    public T GetRequired<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    public FlowStepCommitContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ChatId = ServiceProvider.GetRequiredService<IChatIdProvider>().GetChatId();
    }
}