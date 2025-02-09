
using CurrencyFetch.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace CurrencyFetch.Core.Infrastructure
{
    public class ExchangeMarketHttpClient : IExchangeMarketHttpClient
    {
        private readonly HttpClient _httpClient;
        public ExchangeMarketHttpClient(HttpClient httpClient) {
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
    }
}
