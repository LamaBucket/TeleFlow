namespace TeleFlow.Pipeline;

public interface IHandler<T>
{
    Task Handle(T args);
}