using Telegram.Bot.Extensions.Handlers.Commands.MultiStep;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Factories.CommandFactories;

public class StepCommandFactory : IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>
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