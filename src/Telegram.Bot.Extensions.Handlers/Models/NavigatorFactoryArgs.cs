namespace LisBot.Common.Telegram.Models;

public class NavigatorFactoryArgs
{
    private readonly Dictionary<string, string> _query;

    public string GetParamValue(string key)
    {
        _query.TryGetValue(key, out var value);

        if(value is null)
            throw new Exception("value was null");
        
        return value;
    }

    public NavigatorFactoryArgs(Dictionary<string, string> query)
    {
        _query = query;
    }
}