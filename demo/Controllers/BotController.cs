using TeleFlow;
using TeleFlow.Factories;
using TeleFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace demo.Controllers;

public class BotController : Controller
{
    private readonly UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs> _updateDistributorFactory;

    private IHandler<Update> UpdateDistributor => _updateDistributorFactory.Create();


    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, string webhook, CancellationToken ct)
    {
        var webhookUrl = webhook;
        await bot.SetWebhookAsync(webhookUrl, allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery], cancellationToken: ct);
    
        return $"Webhook set to {webhookUrl}";
    }


    [HttpPost("/botUpdate")]
    public async Task<ActionResult> HandleBotUpdate([FromBody] Update update)
    {
        await UpdateDistributor.Handle(update);

        return Ok();
    }

    public BotController(UpdateDistributorFactory<UpdateDistributorNextHandlerBuildArgs> updateDistributorFactory)
    {
        _updateDistributorFactory = updateDistributorFactory;
    }

}
