using System.Diagnostics;
using LisBot.Common.Telegram;
using LisBot.Common.Telegram.Factories;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace demo.Controllers;

public class BotController : Controller
{
    private readonly UpdateDistributorFactory _updateDistributorFactory;

    private IHandler<Update> UpdateDistributor => _updateDistributorFactory.Create();


    [HttpPost("/botUpdate")]
    public async Task<ActionResult> HandleBotUpdate([FromBody] Update update)
    {
        await UpdateDistributor.Handle(update);

        return Ok();
    }

    public BotController(UpdateDistributorFactory updateDistributorFactory)
    {
        _updateDistributorFactory = updateDistributorFactory;
    }

}
