using LisBot.Common.Telegram.Services;

namespace demo.Services;

public class DemoAuthenticationService : IAuthenticationService
{
    public Task<bool> IsAuthenticated()
    {
        return Task.Run<bool>(() =>
        {
            var random = new Random();

            return random.Next(0, 1) == 1;
        });
    }
}