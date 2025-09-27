using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Exceptions;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using LisBot.Common.Telegram.ViewModels;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public class StateValidationMessageFormatterWithNoButtons<TState>
{
    private bool _awaitsUserResponse;

    private readonly MessageBuilderOptions _messageBuilderOptions;


    private readonly Func<TState, string> _confirmMessageFormatter;

    private readonly string _allGoodButtonDisplayName;

    private readonly string _editButtonDisplayName;


    private int? _editButtonId;


    private readonly CallbackButtonGenerator _buttonGenerator;


    public Message GenerateMessage(TState state)
    {
        if (!_awaitsUserResponse)
        {
            _editButtonId = null;
            _buttonGenerator.StartNewSession();


            MessageBuilder builder = new(_messageBuilderOptions);

            var confirmationMessage = _confirmMessageFormatter(state);

            builder.WithText(confirmationMessage);

            var editButton = GenerateReplyButton(_editButtonDisplayName);
            _editButtonId = editButton.InnerArgs.BID;

            builder
            .WithInlineButton(editButton)
            .WithInlineButton(GenerateAllGoodButton());

            ToggleUserLock();

            return builder.Build();
        }

        throw new InvalidOperationException("Cannot generate message. System awaits for response");
    }


    private ReplyButtonModel<CallbackQueryViewModel> GenerateReplyButton(string displayName)
    {
        return new(_buttonGenerator.GenerateVM(), displayName);
    }

    private ReplyButtonModel<CallbackQueryViewModel> GenerateAllGoodButton()
    {
        return new(_buttonGenerator.GenerateSpecialButton(), _allGoodButtonDisplayName);
    }


    public bool ParseUserResponse(Update args)
    {
        if(!_awaitsUserResponse)
            throw new InvalidOperationException("That Command does not await user response");

        if(args.CallbackQuery is null || args.Type != UpdateType.CallbackQuery)
            throw new InvalidUserInput("This command accepts only callback query");

        var result = ParseCallbackQuery(args.CallbackQuery);

        ToggleUserLock();

        return result;
    }

    private bool ParseCallbackQuery(CallbackQuery query)
    {
        if (string.IsNullOrEmpty(query.Data))
            throw new ArgumentNullException($"the data in {nameof(query)} was null.");

        var btnViewModel = JsonConvert.DeserializeObject<CallbackQueryViewModel>(query.Data);

        if (btnViewModel is null)
            throw new ArgumentException($"Incorrect data in {nameof(query)}");


        if (!_buttonGenerator.IsFromCurrentSession(btnViewModel))
            throw new InvalidUserInput("The button is not from current session");


        if (_buttonGenerator.IsSpecialButton(btnViewModel))
            return true;

        if (btnViewModel.BID != _editButtonId)
            throw new InvalidUserInput("The button is not from current session");

        return false;
    }


    private void ToggleUserLock()
    {
        _awaitsUserResponse = !_awaitsUserResponse;
    }


    public StateValidationMessageFormatterWithNoButtons(MessageBuilderOptions messageBuilderOptions,
                                                        Func<TState, string> confirmMessageFormatter,
                                                        string allGoodButtonDisplayName,
                                                        string editButtonDisplayName)
    {
        _awaitsUserResponse = false;

        _messageBuilderOptions = messageBuilderOptions;

        _confirmMessageFormatter = confirmMessageFormatter;
        _allGoodButtonDisplayName = allGoodButtonDisplayName;

        _buttonGenerator = new();
        _editButtonDisplayName = editButtonDisplayName;
    }
}