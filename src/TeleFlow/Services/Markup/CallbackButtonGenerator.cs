using TeleFlow.ViewModels.CallbackQuery;

namespace TeleFlow.Services.Markup;

public class CallbackButtonGenerator
{
    private int _lastAvailableBtnId;

    private Guid _currentSession;


    public void StartNewSession()
    {
        _lastAvailableBtnId = 0;
        _currentSession = Guid.NewGuid();
    }

    public CallbackQueryViewModel GenerateVM()
    {
        _lastAvailableBtnId += 1;

        return new(_currentSession, _lastAvailableBtnId);
    }

    public CallbackQueryViewModel GenerateSpecialButton()
    {
        return new(_currentSession, 0);
    }


    public bool IsSpecialButton(CallbackQueryViewModel vm)
    {
        return IsFromCurrentSession(vm) && vm.BID == 0;
    }

    public bool IsFromCurrentSession(CallbackQueryViewModel vm)
    {
        return _currentSession == vm.CID;
    }


    public CallbackButtonGenerator()
    {
        _lastAvailableBtnId = 0;
    }
}