using api_clinic.src.Handlers;

namespace api_clinic.src.Requests.Signature
{
    public class SubscribePlanRequest
    {
        public string SignatureId { get; set; } = string.Empty;
        public string ClinicId { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public string Cycle { get; set; } = "monthly";
        public string PaymentMethod { get; set; } = "PIX";
        public AsaasCardData? CardData { get; set; }
    }
}
