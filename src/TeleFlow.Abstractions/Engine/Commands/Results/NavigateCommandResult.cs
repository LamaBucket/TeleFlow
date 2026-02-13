namespace TeleFlow.Abstractions.Engine.Commands.Results;

public class NavigateCommandResult : CommandResult
{
    public string CommandToNavigate { get; init; }

    public NavigateCommandParameters Parameters { get; init; }

    public NavigateCommandResult(string commandToNavigate, Dictionary<string, string>? parameters = null)
    {
        CommandToNavigate = commandToNavigate;
        Parameters = new(parameters ?? []);
    }
}

public class NavigateCommandParameters
{
    private readonly Dictionary<string, string> _parameters;


    public string? GetParameterValue(string paramName)
    {
        return _parameters.GetValueOrDefault(paramName);
    }

    public NavigateCommandParameters(Dictionary<string, string> parameters)
    {
        _parameters = parameters;
    }
}