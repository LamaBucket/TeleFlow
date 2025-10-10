using LisBot.Common.Telegram.Services;

namespace Telegram.Bot.Extensions.Handlers.Services;

public interface IUpdateDistributorArgsBuilder<TBuildArgs> where TBuildArgs : class
{
    TBuildArgs Build(IChatIdProvider chatIdProvider);
}