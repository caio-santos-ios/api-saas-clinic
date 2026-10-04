using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class ProfileEmployee : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("cpf")]
        public string Cpf { get; set; } = string.Empty;

        [BsonElement("registrationNumber")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [BsonElement("hireDate")]
        public DateTime? HireDate { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; } = new();
    }
}
