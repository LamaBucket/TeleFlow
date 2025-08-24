using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram;

public class UpdateDistributor : IHandler<Update>
{
    private readonly Dictionary<long, IHandler<Update>> _chatHandlers;

    private readonly ICommandFactory<UpdateListener, Update, IChatIdProvider> _userHandlerFactory; 

    private readonly Func<IChatIdProvider, IMessageService<string>> _messageServiceFactory; // TODO: TEMP SHIT

    private readonly string[] _commandsToIntercept;

    private readonly string _baseCommand;


    public async Task Handle(Update args)
    {
        await GetNextHandler(args).Handle(args);
    }

    private IHandler<Update> GetNextHandler(Update args)
    {
        var chatIdProvider = new FromUpdateChatIdProvider(args);
        var chatId = chatIdProvider.GetChatId();

        if(!_chatHandlers.ContainsKey(chatId))
        {   
            var updateListener = GenerateUpdateListener(chatIdProvider);
            var interceptor = WrapUpdateListenerInInterceptor(updateListener);
            var exceptionHandler = WrapInterceptorInExceptionHandler(interceptor, chatIdProvider, updateListener, _baseCommand);
            
            _chatHandlers.Add(chatId, exceptionHandler);
        }

        return _chatHandlers[chatId];
    }

    private ExceptionHandler WrapInterceptorInExceptionHandler(UpdateInterceptor interceptor, IChatIdProvider chatIdProvider, INavigatorHandler navigatorHandler, string baseCommand)
    {
        var messageService = _messageServiceFactory.Invoke(chatIdProvider);
        var exceptionHandler = new ExceptionHandler(interceptor, messageService, navigatorHandler, baseCommand);

        return exceptionHandler;
    }

    private UpdateInterceptor WrapUpdateListenerInInterceptor(UpdateListener listener)
    {
        var updateInterceptor = new UpdateInterceptor(listener, _commandsToIntercept);

        return updateInterceptor;
    }

    private UpdateListener GenerateUpdateListener(IChatIdProvider chatIdProvider)
    {
        _userHandlerFactory.SetContext(chatIdProvider);
        return _userHandlerFactory.Create();
    }

    public UpdateDistributor(ICommandFactory<UpdateListener, Update, IChatIdProvider> userHandlerFactory, Func<IChatIdProvider, IMessageService<string>> messageServiceFactory, string[] commandsToIntercept, string baseCommand)
    {
        _chatHandlers = [];
        _userHandlerFactory = userHandlerFactory;
        _messageServiceFactory = messageServiceFactory;
        _commandsToIntercept = commandsToIntercept;
        _baseCommand = baseCommand;
    }
}