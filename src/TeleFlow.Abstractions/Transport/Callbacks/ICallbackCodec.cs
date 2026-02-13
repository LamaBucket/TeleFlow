namespace TeleFlow.Abstractions.Transport.Callbacks;

public interface ICallbackCodec
{
    string EncodeAction(CallbackToken token);
    bool TryDecodeAction(string data, out CallbackToken token);


    //Task<string> EncodePayloadAsync(IServiceProvider sp, object payload, long chatId, int messageId, TimeSpan ttl);
    //Task<(bool ok, T? payload)> TryDecodePayloadAsync<T>(IServiceProvider sp, string data, long chatId, int messageId, bool remove = true);
}