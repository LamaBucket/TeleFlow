namespace LisBot.Common.Telegram.Services;

public abstract class AuthorizationProvider
{
    public abstract bool CanAccess();
}