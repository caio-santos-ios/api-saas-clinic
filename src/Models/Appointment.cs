using api_clinic.src.Models._Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class Appointment : MainModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("patientId")]
        public string PatientId { get; set; } = string.Empty;

        [BsonElement("doctorId")]
        public string DoctorId { get; set; } = string.Empty;

        [BsonElement("procedureId")]
        public string ProcedureId { get; set; } = string.Empty;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [BsonElement("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "AGENDADO";

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; } = 0;
    }
}
