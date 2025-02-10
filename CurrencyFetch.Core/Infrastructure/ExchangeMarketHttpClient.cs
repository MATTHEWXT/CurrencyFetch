
using CurrencyFetch.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace CurrencyFetch.Core.Infrastructure
{
    public class ExchangeMarketHttpClient : IExchangeMarketHttpClient
    {
        private readonly HttpClient _httpClient;
        public ExchangeMarketHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.binance.com");
        }
        public async Task<JObject> GetCurrencyDataAsync(string symbol)
        {
            string uri = $"https://api.binance.com/api/v3/ticker/24hr?symbol={symbol}";

            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            string json = await response.Content.ReadAsStringAsync();
            JObject jsonData = JObject.Parse(json);

            return jsonData;
        }

        public async Task<JArray> GetKlineStatsAsync(string symbol, DateTime startDate, DateTime endDate)
        {
            var startUnix = new DateTimeOffset(startDate).ToUnixTimeMilliseconds();
            var endUnix = new DateTimeOffset(endDate).ToUnixTimeMilliseconds();

            var uri = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval=1d&startTime={startUnix}&endTime={endUnix}";

            HttpResponseMessage response = await _httpClient.GetAsync(uri);
            string json = await response.Content.ReadAsStringAsync();
            JArray jsonData = JArray.Parse(json);

            return jsonData;
        }
    }
}
