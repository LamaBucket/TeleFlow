using Telegram.Bot.Extensions.Handlers.Exceptions;
using Telegram.Bot.Extensions.Handlers.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.Extensions.Handlers.Factories.CommandFactories;

public class UniversalCommandFactory : HandlerFactoryWithArgsBase<string>, IHandlerFactoryWithArgs
{
    private readonly Dictionary<string, IHandlerFactoryWithArgs<ICommandHandler, Update, NavigatorFactoryArgs>> _factories;


    protected override ICommandHandler Create(string args)
    {

        _factories.TryGetValue(GetPathOnly(args), out var factory);

        if(factory is null)
            throw new InvalidUserInput("Command not found.");

        var query = ParseQuery(args);

        factory.SetContext(new(query));

        return factory.Create();
    }

    private static string GetPathOnly(string url)
    {
        return url.Split('?').First();
    }


    private static Dictionary<string, string> ParseQuery(string url)
    {
        var queryArr = url.Split('?');
        
        if(queryArr.Length <= 1)
            return new();

        var query = queryArr[1];

        Dictionary<string, string> result = new();

        foreach(var queryItem in query.Split('&'))
        {
            var queryKeyAndValue = queryItem.Split('=');

            if(queryKeyAndValue.Length != 2)
                throw new ArgumentNullException($"Invalid query in {nameof(url)}");
            
            result.Add(queryKeyAndValue[0], queryKeyAndValue[1]);
        }

        return result;
    }


    public void SetContext(Update args)
    {
        SetContext(GetCommandName(args) ?? throw new ArgumentException($"Unable to retrieve command name from update {nameof(args)}."));
    }

    public virtual string? GetCommandName(Update args)
    {
        return args.GetCommandName();
    }

    public void AddCommandFactory(string commandName, IHandlerFactoryWithArgs<ICommandHandler, Update, NavigatorFactoryArgs> factory)
    {
        if (_factories.ContainsKey(commandName))
            throw new InvalidOperationException("The factory for that command already exists!");

        _factories.Add(commandName, factory);
    }


    public UniversalCommandFactory()
    {
        _factories = new();
    }
}
