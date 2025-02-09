using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyFetch.Core.Models
{
    public class LoadStatus
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid JobId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletionTime { get; set; }
    }
}
