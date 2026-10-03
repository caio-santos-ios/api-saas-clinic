using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class Plan : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;
        
        [BsonElement("status")]
        public string Status { get; set; } = "PENDENTE";

        [BsonElement("cost")]
        public decimal Cost { get; set; } = 0;

        [BsonElement("cycle")]
        public string Cycle { get; set; } = "monthly";

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
    }
}