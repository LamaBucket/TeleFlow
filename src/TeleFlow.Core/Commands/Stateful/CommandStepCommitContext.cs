using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;

namespace TeleFlow.Core.Commands.Stateful;

//This context is provided in OnCommit() Funcs for user's convenience
public class CommandStepCommitContext
{
    public long ChatId { get; init; }

    public IServiceProvider ServiceProvider { get; init; }

    public T GetRequired<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();

    public CommandStepCommitContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ChatId = ServiceProvider.GetRequiredService<IChatIdProvider>().GetChatId();
    }
}