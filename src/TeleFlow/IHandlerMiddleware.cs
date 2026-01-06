namespace TeleFlow;

public interface IHandlerMiddleware<T, TNext> : IHandler<T>
{
    IHandler<TNext> Next { get; }
}

public interface IHandlerMiddleware<T> : IHandlerMiddleware<T, T>
{
    
}