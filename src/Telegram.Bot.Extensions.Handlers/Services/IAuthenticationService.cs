namespace LisBot.Common.Telegram.Services;

public interface IAuthenticationService
{
    Task<bool> IsAuthenticated();
}