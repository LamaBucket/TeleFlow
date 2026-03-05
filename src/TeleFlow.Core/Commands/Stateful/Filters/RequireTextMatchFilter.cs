using System.Text.RegularExpressions;
using TeleFlow.Abstractions.Engine.Commands.Filters;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Core.Commands.Stateful.Filters;

public class RequireTextMatchFilter : ICommandStepFilter
{
    private readonly Regex _regex;

    private readonly string _errorMessage;


    public async Task<CommandStepResult> ExecuteOnUpdate(UpdateContext context, StepHandleDelegate next)
    {
        if(context.Update.Type != UpdateType.Message || context.Update.Message?.Text is null)
            return await next(context);

        var messageText = context.Update.Message.Text;

        if (_regex.IsMatch(messageText))
            return await next(context);
        else
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _errorMessage);
    }

    public Task ExecuteOnEnter(IServiceProvider serviceProvider, StepEnterDelegate next)
        => next(serviceProvider);



    public static RequireTextMatchFilter LettersOnly(string error = "Only letters are allowed")
        => new(@"^[a-zA-Zа-яА-ЯёЁ]+$", error);

    public static RequireTextMatchFilter LettersAndHyphen(string error = "Only letters and '-' are allowed")
        => new(@"^[a-zA-Zа-яА-ЯёЁ\-]+$", error);

    public static RequireTextMatchFilter NumbersOnly(string error = "Only numbers are allowed")
        => new(@"^\d+$", error);

    public static RequireTextMatchFilter NoWhitespace(string error = "Spaces are not allowed")
        => new(@"^\S+$", error);

    public static RequireTextMatchFilter LettersOnlyNoSpaces(string error = "Use letters without spaces")
        => new(@"^[a-zA-Zа-яА-ЯёЁ]+$", error);

    public static RequireTextMatchFilter LettersWithSpaces(string error = "Only letters and spaces are allowed")
        => new(@"^[a-zA-Zа-яА-ЯёЁ\s]+$", error);


    public RequireTextMatchFilter(string pattern, string errorMessage = "Input does not match the required format")
    {
        _regex = new Regex(pattern, RegexOptions.Compiled);
        _errorMessage = errorMessage;
    }
}