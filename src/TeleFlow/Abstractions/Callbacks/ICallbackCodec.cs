using TeleFlow.Models.Callbacks;

namespace TeleFlow.Services.Callbacks;

public interface ICallbackCodec
{
    string EncodeAction(CallbackAction action);
    bool TryDecodeAction(string data, out CallbackAction action);


    //Task<string> EncodePayloadAsync(IServiceProvider sp, object payload, long chatId, int messageId, TimeSpan ttl);
    //Task<(bool ok, T? payload)> TryDecodePayloadAsync<T>(IServiceProvider sp, string data, long chatId, int messageId, bool remove = true);
}