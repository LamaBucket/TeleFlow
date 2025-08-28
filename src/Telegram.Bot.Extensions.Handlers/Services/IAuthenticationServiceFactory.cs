namespace LisBot.Common.Telegram.Services;

public interface IAuthenticationServiceFactory
{
    IAuthenticationService CreateAuthenticationService(long chatId);
}