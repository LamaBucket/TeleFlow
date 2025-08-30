namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandBuildArgs
{
    public UpdateDistributorNextHandlerBuildArgs FromUpdateDistributorArgs { get; init; }

    public INavigatorHandler Navigator { get; init; }


    public UpdateListenerCommandBuildArgs(UpdateDistributorNextHandlerBuildArgs fromUpdateDistributorArgs,
                                          INavigatorHandler navigator)
    {
        FromUpdateDistributorArgs = fromUpdateDistributorArgs;
        Navigator = navigator;
    }
}