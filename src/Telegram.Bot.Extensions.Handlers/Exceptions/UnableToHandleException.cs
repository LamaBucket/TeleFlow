namespace Telegram.Bot.Extensions.Handlers.Exceptions;

public class UnableToHandleException : Exception
{
    public UnableToHandleException() : base("The handler was unable to process the update.")
    {
    }

    public UnableToHandleException(string handlerName, string unableToProcessReason) 
        : base($"The handler '{handlerName}' was unable to process the update. Reason: {unableToProcessReason}")
    {
        
    }

    public UnableToHandleException(string? message) : base(message)
    {
    }

    public UnableToHandleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}