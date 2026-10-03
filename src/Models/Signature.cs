using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_clinic.src.Models
{
    public class Signature : ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("clinicId")]
        public string ClinicId { get; set; } = string.Empty;

        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty;
        
        [BsonElement("status")]
        public string Status { get; set; } = "PENDENTE";

        [BsonElement("paymentMethod")]
        public string PaymentMethod { get; set; } = "credit_card";

        [BsonElement("cycle")]
        public string Cycle { get; set; } = "monthly";

        [BsonElement("value")]
        public decimal Value { get; set; } = 0;

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }

        [BsonElement("nextDueDate")]
        public DateTime? NextDueDate { get; set; }

        [BsonElement("asaasSubscriptionId")]
        public string AsaasSubscriptionId { get; set; } = string.Empty;

        [BsonElement("asaasCustomerId")]
        public string AsaasCustomerId { get; set; } = string.Empty;
    }
}