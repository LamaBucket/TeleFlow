using Telegram.Bot.Types;

namespace TeleFlow.Models.Contexts;

public class SessionContext : UpdateContext
{
    public ChatSession Session { get; init; }


    public SessionContext(ChatSession session, UpdateContext context) : this(session, context.Update, context.ServiceProvider)
    {
        
    }

    public SessionContext(ChatSession session, Update update, IServiceProvider serviceProvider) : base(update, serviceProvider)
    {
        Session = session;
    }
}