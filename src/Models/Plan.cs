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

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("features")]
        public List<string> Features { get; set; } = [];

        [BsonElement("price")]
        public decimal Price { get; set; } = 0;

        [BsonElement("trialDays")]
        public int TrialDays { get; set; }

        [BsonElement("limits")]
        public LimitPlan Limits { get; set; } = new();
    }

    public class LimitPlan
    {
        [BsonElement("maxPatients")]
        public int MaxPatients { get; set; }

        [BsonElement("maxDoctors")]
        public int MaxDoctors { get; set; }
        
        [BsonElement("maxStaff")]
        public int MaxStaff { get; set; }
    }
}