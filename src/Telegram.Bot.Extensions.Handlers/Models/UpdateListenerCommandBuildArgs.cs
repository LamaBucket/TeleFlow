namespace LisBot.Common.Telegram.Models;

public class UpdateListenerCommandBuildArgs<TBuildArgs>
{
    public TBuildArgs FromUpdateDistributorArgs { get; init; }

    public INavigatorHandler Navigator { get; init; }


    public UpdateListenerCommandBuildArgs(TBuildArgs fromUpdateDistributorArgs,
                                          INavigatorHandler navigator)
    {
        FromUpdateDistributorArgs = fromUpdateDistributorArgs;
        Navigator = navigator;
    }
}