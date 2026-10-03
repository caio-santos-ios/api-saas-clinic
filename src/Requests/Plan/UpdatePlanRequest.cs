using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Plan
{
    public class UpdatePlanRequest : Request
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }

        public decimal? Cost { get; set; }

        public string? Cycle { get; set; }

        public string? Description { get; set; }

        public bool? Active { get; set; }
    }
}
