using api_clinic.src.Models._Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class Procedure : MainModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("code")]
        public string Code { get; set; } = string.Empty;

        [BsonElement("durationMinutes")]
        public int DurationMinutes { get; set; } = 30;

        [BsonElement("price")]
        public decimal Price { get; set; } = 0;
    }
}
