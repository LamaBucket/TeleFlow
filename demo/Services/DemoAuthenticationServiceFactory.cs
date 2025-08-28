using demo.Services;

namespace LisBot.Common.Telegram.Services;

public class DemoAuthenticationServiceFactory : IAuthenticationServiceFactory
{
    public IAuthenticationService CreateAuthenticationService(long chatId)
    {
        return new DemoAuthenticationService();
    }
}
