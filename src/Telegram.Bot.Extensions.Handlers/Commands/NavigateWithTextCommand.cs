
using LisBot.Common.Telegram.Services;

namespace LisBot.Common.Telegram.Commands;

public class NavigateWithTextCommand : NavigateCommand
{
    private readonly IMessageService<string> _messageServiceString;

    private readonly string _messageBeforeRedirect;

    protected override async Task Handle()
    {
        await _messageServiceString.SendMessage(_messageBeforeRedirect);
        await base.Handle();
    }

    public NavigateWithTextCommand(INavigatorHandler navHandler, string routeToNavigate, IMessageService<string> messageServiceString, string messageBeforeRedirect) : base(navHandler, routeToNavigate)
    {
        _messageServiceString = messageServiceString;
        _messageBeforeRedirect = messageBeforeRedirect;
    }
}
