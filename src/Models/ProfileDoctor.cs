using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class ProfileDoctor : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("specialty")]
        public string Specialty { get; set; } = string.Empty;

        [BsonElement("licenseNumber")]
        public string LicenseNumber { get; set; } = string.Empty;

        [BsonElement("licenseState")]
        public string LicenseState { get; set; } = string.Empty;

        [BsonElement("bio")]
        public string Bio { get; set; } = string.Empty;

        [BsonElement("address")]
        public Address Address { get; set; } = new();
    }
}