using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Filters;

public class GoNextCallbackFilter : ICommandStepFilter
{
    public async Task<CommandStepResult> ExecuteOnUpdate(UpdateContext context, StepHandleDelegate next)
    {
        var callbackCodec = context.GetService<ICallbackCodec>();
        var callbackActionParser = context.GetService<ICallbackActionParser>();

        var update = context.Update;
        
        if(update.Type == UpdateType.CallbackQuery && update.CallbackQuery is not null && update.CallbackQuery.Data is not null)
        {
            var callbackData = update.CallbackQuery.Data;

            if(callbackCodec.TryDecodeAction(callbackData, out var token))
            {
                if(callbackActionParser.TryParse(token, out var action))
                {
                    if(action is CallbackAction.StepAction.Finish)
                        return CommandStepResult.Next;
                }
            }   
        }

        return await next(context);
    }

    public Task ExecuteOnEnter(IServiceProvider serviceProvider, StepEnterDelegate next)
        => next(serviceProvider);
}

