namespace TeleFlow.Models.CommandResults;

public class NavigateCommandResult : CommandResult
{
    public string CommandToNavigate { get; init; }

    private readonly Dictionary<string, string> _parameters;


    public string? GetParameterValue(string paramName)
    {
        return _parameters.GetValueOrDefault(paramName);
    }


    public NavigateCommandResult(string commandToNavigate, Dictionary<string, string> parameters)
    {
        CommandToNavigate = commandToNavigate;
        _parameters = parameters;
    }
}