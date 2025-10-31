namespace TeleFlow;

public interface IHandler<T>
{
    Task Handle(T args);
}