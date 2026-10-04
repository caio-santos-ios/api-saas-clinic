using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.Plan
{
    public class UpdatePlanRequest : Request
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? Description { get; set; }

        public List<string>? Features { get; set; }

        public decimal? Price { get; set; }

        public int? TrialDays { get; set; }

        public LimitPlan? Limits { get; set; }

        public bool? Active { get; set; }
    }
}
