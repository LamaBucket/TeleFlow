using TeleFlow.Commands;

namespace TeleFlow.Commands.Factories;

public interface ICommandFactory<out TCommand, TContext> where TCommand : ICommandHandler
{
    TCommand Create(TContext context);
}