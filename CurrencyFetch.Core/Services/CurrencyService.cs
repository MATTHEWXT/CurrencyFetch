using CurrencyFetch.Core.Infrastructure;
using CurrencyFetch.Core.Interfaces;
using CurrencyFetch.Core.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace CurrencyFetch.Core.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeMarketHttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(IExchangeMarketHttpClient httpClient, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task InsertCurrencyDataAsync(string symbol, DateTime startDate, DateTime endDate)
        {
            Guid jobId = Guid.NewGuid();
            LoadStatus newStatus = new LoadStatus
            {
                Id = ObjectId.GenerateNewId(),
                JobId = jobId,
                Status = "In processing",
            };

            try
            {
                await InsertLoadStatusDataAsync(newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting LoadStatus into MongoDB.");
                return;
            }

            JObject dataObject = new JObject
            {
                ["jobId"] = jobId.ToString(),
            };

            try
            {
                JToken statistics;
                if (startDate.Day == endDate.Day)
                {
                    statistics = await _httpClient.GetCurrencyDataAsync(symbol);
                }
                else
                {
                    statistics = await _httpClient.GetKlineStatsAsync(symbol, startDate, endDate);
                }

                if (statistics == null || statistics.Type == JTokenType.Null)
                {
                    throw new Exception("Received null or invalid response from Binance API.");
                }

                dataObject["statistics"] = statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from Binance API.");
                newStatus.CompletionTime = DateTime.UtcNow;
                newStatus.Status = "Data fetch error";
                await UpdateLoadStatusDataAsync(newStatus);
                return;
            }

            try
            {
                var bsonData = BsonDocument.Parse(dataObject.ToString());
                var collection = MongoDbService.GetCollection<BsonDocument>("CurrencyStatistics");

                await collection.InsertOneAsync(bsonData);

                newStatus.CompletionTime = DateTime.UtcNow;
                newStatus.Status = "Success";
                await UpdateLoadStatusDataAsync(newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting data into MongoDB.");
                newStatus.CompletionTime = DateTime.UtcNow;
                newStatus.Status = "Insertion error";
                await UpdateLoadStatusDataAsync(newStatus);
            }
        }


        private async Task InsertLoadStatusDataAsync(LoadStatus status)
        {
            var collection = MongoDbService.GetCollection<LoadStatus>("LoadStatuses");

            await collection.InsertOneAsync(status);
        }

        private async Task UpdateLoadStatusDataAsync(LoadStatus newStatus)
        {
            var collection = MongoDbService.GetCollection<LoadStatus>("LoadStatuses");
            var filter = Builders<LoadStatus>.Filter.Eq(ls => ls.Id, newStatus.Id);

            await collection.UpdateOneAsync(filter, Builders<LoadStatus>.Update
                .Set(ls => ls.Status, newStatus.Status)
                .Set(ls => ls.CompletionTime, newStatus.CompletionTime));
        }
    }
}
