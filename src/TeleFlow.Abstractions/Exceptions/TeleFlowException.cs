namespace TeleFlow.Abstractions.Exceptions;

public class TeleFlowException : Exception
{
    public TeleFlowException(string message) : base(message)
    {
        
    }

    public TeleFlowException(string message, Exception inner) : base(message, inner)
    {
        
    }
}