using Newtonsoft.Json;

namespace Telegram.Bot.Extensions.Handlers.ViewModels.CallbackQuery;

public class CallbackQueryViewModel
{
    public Guid CID { get; init; }

    public int BID { get; init; }


    [JsonConstructor]
    public CallbackQueryViewModel(Guid cId, int bId)
    {
        CID = cId;
        BID = bId;
    }
}