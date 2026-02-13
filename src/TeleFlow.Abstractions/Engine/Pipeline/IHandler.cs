namespace TeleFlow.Abstractions.Engine.Pipeline;

public interface IHandler<T>
{
    Task Handle(T args);
}