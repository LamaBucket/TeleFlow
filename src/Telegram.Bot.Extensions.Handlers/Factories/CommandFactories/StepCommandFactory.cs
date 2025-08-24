using LisBot.Common.Telegram.Commands.MultiStep;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Factories.CommandFactories;

public class StepCommandFactory : ICommandFactory<StepCommand, Update, StepCommand>
{
    private StepCommand? _nextCommand;

    private Func<StepCommand?, StepCommand> _lambdaFactory;

    public StepCommand Create()
    {
        var stepCommand = _lambdaFactory(_nextCommand);

        _nextCommand = null;

        return stepCommand;
    }

    public void SetContext(StepCommand args)
    {
        _nextCommand = args;
    }

    public StepCommandFactory(Func<StepCommand?, StepCommand> lambdaFactory)
    {
        _lambdaFactory = lambdaFactory;
    }
}