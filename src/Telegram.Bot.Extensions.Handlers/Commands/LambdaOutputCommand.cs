using LisBot.Common.Telegram.Commands;

namespace Telegram.Bot.Extensions.Handlers.Commands;

public class LambdaOutputCommand : OutputCommand
{
    private readonly Func<Task> _lambdaFunc;

    protected override Task Handle()
    {
        return _lambdaFunc.Invoke();
    }

    public LambdaOutputCommand(Func<Task> lambdaFunc)
    {
        _lambdaFunc = lambdaFunc;
    }
}