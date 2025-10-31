namespace TeleFlow.Commands;

public class NavigateCommand : OutputCommand
{
    public INavigatorHandler NavHandler { get; init; }

    public string RouteToNavigate { get; init; }

    protected override async Task Handle()
    {
        await NavHandler.Handle(RouteToNavigate);
    }

    public NavigateCommand(INavigatorHandler navHandler, string routeToNavigate)
    {
        NavHandler = navHandler;
        RouteToNavigate = routeToNavigate;
    }
}
