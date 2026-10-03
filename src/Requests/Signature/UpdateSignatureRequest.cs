using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Signature
{
    public class UpdateSignatureRequest : Request
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        public string Id { get; set; } = string.Empty;

        public string? PlanId { get; set; }

        public string? Status { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Cycle { get; set; }

        public decimal? Value { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? NextDueDate { get; set; }

        public string? AsaasSubscriptionId { get; set; }

        public string? AsaasCustomerId { get; set; }

        public bool? Active { get; set; }
    }
}
