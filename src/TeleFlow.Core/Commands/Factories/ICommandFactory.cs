using TeleFlow.Abstractions.Engine.Commands;

namespace TeleFlow.Core.Commands.Factories;

public interface ICommandFactory<out TCommand, TContext> where TCommand : ICommandHandler
{
    TCommand Create(TContext context);
}