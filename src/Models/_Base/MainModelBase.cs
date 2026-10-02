using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models._Base
{
    public class MainModelBase : ModelBase
    {
        [BsonElement("clinicId")]
        public string ClinicId { get; set; } = string.Empty;
    }
}