using Telegram.Bot.Extensions.Handlers.Commands.MultiStep;
using Telegram.Bot.Extensions.Handlers.Exceptions;
using Telegram.Bot.Extensions.Handlers.Factories.CommandFactories;
using Telegram.Bot.Extensions.Handlers.Services;
using Telegram.Bot.Extensions.Handlers.ViewModels;
using Telegram.Bot.Extensions.Handlers.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Extensions.Handlers.Services.Messaging;

public class StateValidationMessageFormatter<TState> 
{
    private bool _awaitsUserResponse;

    private readonly MessageBuilderOptions _messageBuilderOptions;


    private readonly Func<TState, string> _confirmMessageFormatter;

    private readonly IEnumerable<Tuple<IStateValidationDisplayNameProvider, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> _btnDisplayNameProviders;

    private readonly string _allGoodButtonDisplayName;
     

    private readonly Dictionary<int, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>> _lastSessionDisplayButtonContext;


    private readonly CallbackButtonGenerator _buttonGenerator;


    public Message GenerateMessage(TState state)
    {
        if(!_awaitsUserResponse)
        {
            _lastSessionDisplayButtonContext.Clear();
            _buttonGenerator.StartNewSession();


            MessageBuilder builder = new(_messageBuilderOptions);

            var confirmationMessage = _confirmMessageFormatter(state);

            builder.WithText(confirmationMessage);

            foreach(var tuple in _btnDisplayNameProviders)
            {
                var displayNameProvider = tuple.Item1;
                var stepFactory = tuple.Item2;

                var replyButton = GenerateReplyButton(displayNameProvider);
                var buttonId = replyButton.InnerArgs.BID;

                builder.WithInlineButton(replyButton);
                _lastSessionDisplayButtonContext.Add(buttonId, stepFactory);
            }

            builder.WithInlineButton(GenerateAllGoodButton());

            ToggleUserLock();

            return builder.Build();
        }

        throw new InvalidOperationException("Cannot generate message. System awaits for response");
    }

    private ReplyButtonModel<CallbackQueryViewModel> GenerateReplyButton(IStateValidationDisplayNameProvider displayNameProvider)
    {
        return new(_buttonGenerator.GenerateVM(), displayNameProvider.GetDisplayName());
    }

    private ReplyButtonModel<CallbackQueryViewModel> GenerateAllGoodButton()
    {
        return new(_buttonGenerator.GenerateSpecialButton(), _allGoodButtonDisplayName);
    }


    public IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>? ParseUserResponse(Update args)
    {
        if(!_awaitsUserResponse)
            throw new InvalidOperationException("That Command does not await user response");

        if(args.CallbackQuery is null || args.Type != UpdateType.CallbackQuery)
            throw new InvalidUserInput("This command accepts only callback query");

        var result = ParseCallbackQuery(args.CallbackQuery);

        ToggleUserLock();

        return result;
    }

    private IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>? ParseCallbackQuery(CallbackQuery query)
    {
        if(string.IsNullOrEmpty(query.Data))
            throw new ArgumentNullException($"the data in {nameof(query)} was null.");

        var btnViewModel = JsonConvert.DeserializeObject<CallbackQueryViewModel>(query.Data);

        if(btnViewModel is null)
            throw new ArgumentException($"Incorrect data in {nameof(query)}");

        
        if(!_buttonGenerator.IsFromCurrentSession(btnViewModel))
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


    public StateValidationMessageFormatter(MessageBuilderOptions messageBuilderOptions, Func<TState, string> confirmMessageFormatter, IEnumerable<Tuple<IStateValidationDisplayNameProvider, IHandlerFactoryWithArgs<StepCommand, Update, StepCommand>>> btnDisplayNameProviders, string allGoodButtonDisplayName)
    {
        _awaitsUserResponse = false;

        _messageBuilderOptions = messageBuilderOptions;
        
        _confirmMessageFormatter = confirmMessageFormatter;
        _btnDisplayNameProviders = btnDisplayNameProviders;
        _allGoodButtonDisplayName = allGoodButtonDisplayName;
        
        _lastSessionDisplayButtonContext = new();
        _buttonGenerator = new();
    }
}