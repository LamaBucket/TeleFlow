using TeleFlow.Abstractions.Exceptions;

namespace TeleFlow.Core.Exceptions;

public class TeleFlowPipelineException : TeleFlowException
{
    public TeleFlowPipelineException(string message) : base(message)
    {
    }

    public TeleFlowPipelineException(string message, Exception inner) : base(message, inner)
    {
    }
}