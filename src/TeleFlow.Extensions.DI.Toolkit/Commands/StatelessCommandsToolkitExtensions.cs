using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateless;
using TeleFlow.Extensions.DI.Builders.Commands;

namespace TeleFlow.Extensions.DI.Toolkit.Commands;

public static partial class CommandRouterToolkitExtensions
{
    public static CommandFilterBuilder AddSendMessage(this CommandRouterBuilder commands, string commandName, string message)
    => commands.Add(commandName, () => new SendMessageCommand(message));

    public static CommandFilterBuilder AddSendMessage(this CommandRouterBuilder commands, string commandName, InlineMarkupMessage message)
        => commands.Add(commandName, () => new SendMessageCommand(message));

    public static CommandFilterBuilder AddSendMessage(this CommandRouterBuilder commands, string commandName, MessageFactory messageFactory)
        => commands.Add(commandName, () => new SendMessageCommand(messageFactory));
    


    public static CommandFilterBuilder AddNavigateWithText(this CommandRouterBuilder commands, string commandName, string text, string navigateTo, Dictionary<string, string>? args = null)
        => commands.Add(commandName, () => new NavigateWithTextCommand(text, navigateTo, args != null ? new(args) : null));
    

    public static CommandFilterBuilder AddNavigate(this CommandRouterBuilder commands, string commandName, string navigateTo, Dictionary<string, string>? args = null)
        => commands.Add(commandName, () => new NavigateCommand(navigateTo, args != null ? new(args) : null));

}
