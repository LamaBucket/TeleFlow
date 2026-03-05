using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Abstractions.Transport.Messaging;
using TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render.Internal;
using TeleFlow.Core.Transport.Callbacks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateSelection.Render;

public class DateSelectionRenderService : IStepRenderService<DateSelectionStepData>
{
    private delegate InlineKeyboardMarkup RenderMarkupDelegate(Func<CallbackAction, string> markupButtonActionCodec, DateSelectionStepData model, DateSelectionStepDataConstraints constraints, DateSelectionRenderServiceOptions config);


    private readonly DateSelectionStepDataConstraints _constraints;

    private readonly DateSelectionRenderServiceOptions _config;


    public InlineMarkupMessage Render(IServiceProvider serviceProvider, DateSelectionStepData model)
    {
        if(model.DateSelectionCompleted)
            return RenderCompleted(serviceProvider, model, _config);
        
        return RenderIncomplete(serviceProvider, model, _constraints, _config);
    }

    private static InlineMarkupMessage RenderCompleted(IServiceProvider serviceProvider, DateSelectionStepData model, DateSelectionRenderServiceOptions config)
        => new(){ Text = config.UserPrompt(serviceProvider, model), Markup = null};

    private static InlineMarkupMessage RenderIncomplete(IServiceProvider serviceProvider, DateSelectionStepData model, DateSelectionStepDataConstraints constraints, DateSelectionRenderServiceOptions config)
    {
        RenderMarkupDelegate markupRenderFunction = model.Page switch
        {
            DateSelectionStepPage.YearSelection  => YearSelectionRenderService.RenderMarkup,
            DateSelectionStepPage.MonthSelection => MonthSelectionRenderService.RenderMarkup,
            DateSelectionStepPage.DaySelection   => DaySelectionRenderService.RenderMarkup,
            _ => throw new Exception($"Unsupported date input page: {model.Page}. This")
        };
        
        var callbackCodec = serviceProvider.GetRequiredService<ICallbackCodec>();
        var actionParser  = serviceProvider.GetRequiredService<ICallbackActionParser>();

        var markupButtonActionCodec = (CallbackAction action) =>
        {
            return callbackCodec.EncodeAction(actionParser.Parse(action));
        };

        return new()
        {
            Text = config.UserPrompt(serviceProvider, model),
            Markup = markupRenderFunction(markupButtonActionCodec, model, constraints, config)
        };
    }

    public DateSelectionRenderService(DateSelectionStepDataConstraints constraints, DateSelectionRenderServiceOptions config)
    {
        _constraints = constraints;
        _config = config;
    }
}