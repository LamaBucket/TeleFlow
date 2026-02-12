namespace TeleFlow.Abstractions.Transport.Callbacks;

public class CallbackToken
{
    public string Kind { get; init; }

    public string Data { get; init; }

    public CallbackToken(string kind, string data)
    {
        Kind = kind;
        Data = data;
    }
}