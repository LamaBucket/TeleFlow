using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Factories.CommandFactories;
using Telegram.Bot.Types;

namespace LisBot.Common.Telegram.Services;

public interface IStateValidationDisplayNameProvider
{
    IHandlerFactoryWithArgs<StepCommand, Update, StepCommand> StepToRestartCommandFactory { get; }
    
    string GetDisplayName();
}