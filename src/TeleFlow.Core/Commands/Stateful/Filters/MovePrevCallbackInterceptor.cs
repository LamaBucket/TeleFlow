using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Transport.Callbacks;
using TeleFlow.Core.Transport.Markup;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Filters;

public class MovePrevCallbackFilter : ICommandStepFilter
{

    public async Task<CommandStepResult> ExecuteOnUpdate(UpdateContext context, StepHandleDelegate next)
    {
        var goBackButtonResult = TryHandleGoBackButton(context);

        if(goBackButtonResult is not null)
            return goBackButtonResult;
        
        return await next(context);
    }

    public static CommandStepResult? TryHandleGoBackButton(UpdateContext context)
    {
        if(context.Update.Type == UpdateType.CallbackQuery && context.Update.CallbackQuery is not null)
        {
            var query = context.Update.CallbackQuery;

            if(query.Data is not null)
            {
                var serviceProvider = context.ServiceProvider;

                var actionParser = serviceProvider.GetRequiredService<ICallbackActionParser>();
                var callbackCodec = serviceProvider.GetRequiredService<ICallbackCodec>();

                if(callbackCodec.TryDecodeAction(query.Data, out var token))
                {
                    if(actionParser.TryParse(token, out var action))
                    {
                        if(action is CallbackAction.StepAction.Back)
                        {
                            return CommandStepResult.Previous;
                        }
                    }
                }
            }
        }

        return null;
    }


    public async Task ExecuteOnEnter(IServiceProvider serviceProvider, StepEnterDelegate next)
    {
        ApplyMessageTemplate(serviceProvider);
        await next(serviceProvider);
        ClearMessageTemplate(serviceProvider);
    }

    public static void ApplyMessageTemplate(IServiceProvider serviceProvider)
    {
        var actionParser = serviceProvider.GetRequiredService<ICallbackActionParser>();
        var callbackCodec = serviceProvider.GetRequiredService<ICallbackCodec>();

        var templateService = serviceProvider.GetRequiredService<IMessageSenderTemplateService>();

        templateService.ApplyTemplate((message) =>
        {
            if(message.ReplyMarkup is InlineKeyboardMarkup markup)
            {
                var token = callbackCodec.EncodeAction(actionParser.Parse(CallbackActions.Step.Back));
                markup.AddNewRow(new InlineKeyboardButton("Go Prev"){ CallbackData = token });
            }

            return message;
        });
    }

    public static void ClearMessageTemplate(IServiceProvider serviceProvider)
    {
        var templateService = serviceProvider.GetRequiredService<IMessageSenderTemplateService>();
        templateService.ClearTemplates();
    }

}