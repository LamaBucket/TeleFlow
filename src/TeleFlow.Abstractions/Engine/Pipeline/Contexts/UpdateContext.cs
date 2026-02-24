using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace TeleFlow.Abstractions.Engine.Pipeline.Contexts;

public class UpdateContext
{
    public Update Update { get; init; }

    public IServiceProvider ServiceProvider { get; init; }

    public TService GetService<TService>()
        where TService : notnull
    {
        return ServiceProvider.GetRequiredService<TService>();
    }

    public UpdateContext(Update update, IServiceProvider serviceProvider)
    {
        Update = update;
        ServiceProvider = serviceProvider;
    }
}