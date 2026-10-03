using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Signature
{
    public class CreateSignatureRequest : Request
    {
        [Required(ErrorMessage = "O ClinicId é obrigatório.")]
        public string ClinicId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O PlanId é obrigatório.")]
        public string PlanId { get; set; } = string.Empty;

        public string Status { get; set; } = "PENDENTE";

        public string PaymentMethod { get; set; } = "credit_card";

        public string Cycle { get; set; } = "monthly";

        [Required(ErrorMessage = "O Valor é obrigatório.")]
        public decimal Value { get; set; } = 0;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public DateTime? NextDueDate { get; set; }

        public string AsaasSubscriptionId { get; set; } = string.Empty;

        public string AsaasCustomerId { get; set; } = string.Empty;
    }
}
