using CurrencyFetch.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CurrencyFetch.Core.Infrastructure
{
    public class MongoDbService
    {
        private static IMongoDatabase _database = null!;
        private static string _databaseName = null!;
        public static void SetConnection(string connectionString, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException(nameof(databaseName), "Database name cannot be null or empty.");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _databaseName = databaseName;
        }

        public static IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (_database == null)
                throw new InvalidOperationException("MongoDB connection is not established. Please call SetUpConnection first.");

            return _database.GetCollection<T>(collectionName);
        }
        public static async Task<LoadStatus> GetLoadStatus(Guid jobId, string collectionName)
        {
            var statusCollection = _database?.GetCollection<LoadStatus>(collectionName);

            var filter = Builders<LoadStatus>.Filter.Eq(ls => ls.JobId, jobId);
            var status = await statusCollection.Find(filter).FirstOrDefaultAsync();

            return status;
        }
    }
}
