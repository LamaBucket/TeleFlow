using TeleFlow.Exceptions;
using TeleFlow.ViewModels;
using TeleFlow.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using TeleFlow.Services.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeleFlow.Services.Messaging;

public class ListValueSelectionMessageFormatter<TEnumerable>  where TEnumerable : class
{
    private bool CanHaveNoResult => _noSelectButtonName is not null;


    private bool _awaitsUserResponse;

    private readonly Dictionary<int, TEnumerable> _lastSessionDisplayButtonContext;

    private readonly MessageBuilderOptions _messageBuilderOptions;


    private readonly string _onCommandCreatedMessage;

    private readonly string? _noSelectButtonName;

    private readonly IEnumerable<Tuple<string, TEnumerable>> _btnDisplayNameProviders;


    private readonly CallbackButtonGenerator _buttonGenerator;


    public Message GenerateMessage()
    {
        if(!_awaitsUserResponse)
        {
            _lastSessionDisplayButtonContext.Clear();
            _buttonGenerator.StartNewSession();

            MessageBuilder builder = new(_messageBuilderOptions);

            builder.WithText(_onCommandCreatedMessage);

            foreach(var tuple in _btnDisplayNameProviders)
            {
                var displayNameProvider = tuple.Item1;
                var enumValue = tuple.Item2;

                var replyButton = GenerateReplyButton(displayNameProvider);
                var buttonId = replyButton.InnerArgs.BID;

                builder.WithInlineButton(replyButton);
                _lastSessionDisplayButtonContext.Add(buttonId, enumValue);
            }

            if(CanHaveNoResult)
                builder.WithInlineButton(GenerateNoSelectButton());

            ToggleUserLock();

            return builder.Build();
        }

        throw new InvalidOperationException("Cannot generate message. System awaits for response");
    }

    private ReplyButtonModel<CallbackQueryViewModel> GenerateReplyButton(string displayName)
    {
        return new(_buttonGenerator.GenerateVM(), displayName);
    }

    private ReplyButtonModel<CallbackQueryViewModel> GenerateNoSelectButton()
    {
        if(_noSelectButtonName is null)
            throw new NullReferenceException(nameof(_noSelectButtonName));

        return new(_buttonGenerator.GenerateSpecialButton(), _noSelectButtonName);
    }


    public TEnumerable ParseUserResponseCannotBeEmpty(Update args)
    {
        var result = ParseUserResponse(args);

        if(result is not null)
        {
            return result;
        }
        else
        {
            if(CanHaveNoResult)
                throw new ArgumentNullException($"This method is intended to work without possible null {nameof(result)}.");
            else
                throw new NullReferenceException($"The {nameof(result)} was null, which is not inteded");
        }
    }

    public TEnumerable? ParseUserResponse(Update args)
    {
        if(!_awaitsUserResponse)
            throw new InvalidOperationException("That Command does not await user response");

        if(args.CallbackQuery is null || args.Type != UpdateType.CallbackQuery)
            throw new InvalidUserInput("This command accepts only the button input");

        var result = ParseCallbackQuery(args.CallbackQuery);

        ToggleUserLock();

        return result;
    }

    private TEnumerable? ParseCallbackQuery(CallbackQuery query)
    {
        if(string.IsNullOrEmpty(query.Data))
            throw new ArgumentException($"No data in {nameof(query)}");

        var btnViewModel = JsonConvert.DeserializeObject<CallbackQueryViewModel>(query.Data) ?? throw new ArgumentException($"The data in {nameof(query)} was in incorrect format.");

        if (!_buttonGenerator.IsFromCurrentSession(btnViewModel))
            throw new InvalidUserInput("The button is not from current session");

        
        if(_buttonGenerator.IsSpecialButton(btnViewModel))
            return null;

        _lastSessionDisplayButtonContext.TryGetValue(btnViewModel.BID, out var factory);

        if(factory is null)
            throw new ArgumentException("The Button UID was not found");

        return factory;
    }


    private void ToggleUserLock()
    {
        _awaitsUserResponse = !_awaitsUserResponse;
    }


    public ListValueSelectionMessageFormatter(MessageBuilderOptions messageBuilderOptions, IEnumerable<Tuple<string, TEnumerable>> btnDisplayNameProviders, string onCommandCreatedMessage, string? noSelectButtonName = null)
    {
        _awaitsUserResponse = false;
        _lastSessionDisplayButtonContext = new();

        _messageBuilderOptions = messageBuilderOptions;

        _btnDisplayNameProviders = btnDisplayNameProviders;
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _noSelectButtonName = noSelectButtonName;

        _buttonGenerator = new();
    }

    public ListValueSelectionMessageFormatter(MessageBuilderOptions messageBuilderOptions, IEnumerable<TEnumerable> values, Func<TEnumerable, string> enumFormatter, string onCommandCreatedMessage, string? noSelectButtonName = null)
    {
        _awaitsUserResponse = false;
        _lastSessionDisplayButtonContext = new();

        _messageBuilderOptions = messageBuilderOptions;
        
        _onCommandCreatedMessage = onCommandCreatedMessage;
        _noSelectButtonName = noSelectButtonName;

        var btnDisplayNameProviders = new List<Tuple<string, TEnumerable>>();
        
        foreach(var value in values)
        {
            btnDisplayNameProviders.Add(new(enumFormatter.Invoke(value), value));
        }

        _btnDisplayNameProviders = btnDisplayNameProviders;

        _buttonGenerator = new();
    }

}