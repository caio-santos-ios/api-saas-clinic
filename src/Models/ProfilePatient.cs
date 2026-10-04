using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class ProfilePatient : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("rg")]
        public string Rg { get; set; } = string.Empty;

        [BsonElement("birthDate")]
        public DateTime? BirthDate { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("bloodType")]
        public string BloodType { get; set; } = string.Empty;

        [BsonElement("allergies")]
        public List<string> Allergies { get; set; } = [];

        [BsonElement("emergencyContactName")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [BsonElement("emergencyContactPhone")]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [BsonElement("emergencyContactRelationship")]
        public string EmergencyContactRelationship { get; set; } = string.Empty;

        [BsonElement("address")]
        public Address Address { get; set; } = new();

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }
}
