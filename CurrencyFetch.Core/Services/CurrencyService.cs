using CurrencyFetch.Core.Infrastructure;
using CurrencyFetch.Core.Interfaces;
using CurrencyFetch.Core.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace CurrencyFetch.Core.Services
{
    public class CurrencyService
    {
        private readonly IExchangeMarketHttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(IExchangeMarketHttpClient httpClient, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task InsertCurrencyDataAsync(string symbol)
        {
            JObject marketData = await _httpClient.GetCurrencyDataAsync(symbol);

            Guid jobId = Guid.NewGuid();
            marketData["jobId"] = jobId.ToString();

            var bsonData = BsonDocument.Parse(marketData.ToString());
            var colection = MongoDbService.GetCollection<BsonDocument>("CurrencyStatistics");

            LoadStatus newStatus = new LoadStatus
            {
                Id = ObjectId.GenerateNewId(),
                JobId = jobId,
                Status = "In procesing",
            };

            try
            {
                await InsertLoadStatusDataAsync(newStatus);
                await colection.InsertOneAsync(bsonData);

                newStatus.CompletionTime = DateTime.UtcNow;
                newStatus.Status = "Success";
                await UpdateLoadStatusDataAsync(newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when inserting CurrencyStatistics data into MongoDB");

                newStatus.CompletionTime = DateTime.UtcNow;
                newStatus.Status = "Insertion error";
                await UpdateLoadStatusDataAsync(newStatus);
            }
        }

        public static async Task InsertLoadStatusDataAsync(LoadStatus status)
        {
            var collection = MongoDbService.GetCollection<LoadStatus>("LoadStatuses");

            await collection.InsertOneAsync(status);
        }

        public async Task UpdateLoadStatusDataAsync(LoadStatus newStatus)
        {
            var collection = MongoDbService.GetCollection<LoadStatus>("LoadStatuses");
            var filter = Builders<LoadStatus>.Filter.Eq(ls => ls.Id, newStatus.Id);

            await collection.UpdateOneAsync(filter, Builders<LoadStatus>.Update
                .Set(ls => ls.Status, newStatus.Status)
                .Set(ls => ls.CompletionTime, newStatus.CompletionTime));
        }
    }
}
