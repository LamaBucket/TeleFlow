using System.Diagnostics;
using LisBot.Common.Telegram.Factories;
using Microsoft.AspNetCore.Mvc;

namespace demo.Controllers;

public class BotController : Controller
{
    private readonly UpdateDistributorFactory _updateDistributorFactory;


    public BotController(UpdateDistributorFactory updateDistributorFactory)
    {
        _updateDistributorFactory = updateDistributorFactory;
    }

}
