using Newtonsoft.Json.Linq;

namespace CurrencyFetch.Core.Interfaces
{
    public interface IExchangeMarketHttpClient
    {
        Task<JObject> GetCurrencyDataAsync(string symbol);
        Task<JArray> GetKlineStatsAsync(string symbol, DateTime startDate, DateTime endDate);
    }
}