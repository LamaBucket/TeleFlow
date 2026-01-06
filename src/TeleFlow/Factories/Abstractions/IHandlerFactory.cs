namespace TeleFlow.Factories;

public interface IHandlerFactory<out THandler, TArgs> where THandler : IHandler<TArgs>
{
    THandler Create();
}