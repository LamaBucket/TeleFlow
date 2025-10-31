using TeleFlow.Services;

namespace TeleFlow.Services;

public interface IUpdateDistributorArgsBuilder<TBuildArgs> where TBuildArgs : class
{
    TBuildArgs Build(IChatIdProvider chatIdProvider);
}