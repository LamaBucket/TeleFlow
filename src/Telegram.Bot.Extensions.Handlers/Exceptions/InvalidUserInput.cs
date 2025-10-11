
namespace Telegram.Bot.Extensions.Handlers.Exceptions;

public class InvalidUserInput : UserException
{
    public InvalidUserInput()
    {
    }

    public InvalidUserInput(string? message) : base(message)
    {
    }

    public InvalidUserInput(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}