using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class Clinic : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [BsonElement("tradeName")]
        public string TradeName { get; set; } = string.Empty;

        [BsonElement("corporateName")]
        public string CorporateName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string Phone { get; set; } = string.Empty;

        [BsonElement("address")]
        public AddressClinic Address { get; set; } = new();

        [BsonElement("setting")]
        public SettingClinic Setting { get; set; } = new();
    }

    public class AddressClinic
    {
        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;

        [BsonElement("street")]
        public string Street { get; set; } = string.Empty;

        [BsonElement("number")]
        public string Number { get; set; } = string.Empty;

        [BsonElement("complement")]
        public string Complement { get; set; } = string.Empty;

        [BsonElement("neighborhood")]
        public string Neighborhood { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("state")]
        public string State { get; set; } = string.Empty;
    }
    public class SettingClinic
    {
        [BsonElement("logo")]
        public string Logo { get; set; } = string.Empty;

        [BsonElement("primaryColor")]
        public string PrimaryColor { get; set; } = string.Empty;

        [BsonElement("secondaryColor")]
        public string SecondaryColor { get; set; } = string.Empty;
    }
}