using TeleFlow.Abstractions.Transport.Callbacks;

namespace TeleFlow.Core.Transport.Callbacks;

public interface ICallbackActionParser
{
    CallbackToken Parse(CallbackAction action);

    bool TryParse(CallbackToken token, out CallbackAction action);
}