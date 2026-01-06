using TeleFlow.Factories.CommandFactories;
using TeleFlow.Models;
using Telegram.Bot.Types;

namespace TeleFlow.Factories;

public class UpdateListenerFactory<TBuildArgs> : IHandlerFactoryWithContext<UpdateListener, Update, TBuildArgs> where TBuildArgs : class
{
    
}