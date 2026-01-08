using TeleFlow.Commands;

namespace TeleFlow.Factories;

public interface ICommandFactory<out TCommand, TContext> where TCommand : ICommandHandler
{
    TCommand Create(TContext context);
}