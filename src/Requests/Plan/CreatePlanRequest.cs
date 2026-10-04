using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.Plan
{
    public class CreatePlanRequest : Request
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<string> Features { get; set; } = [];

        [Required(ErrorMessage = "O Preço é obrigatório.")]
        public decimal Price { get; set; } = 0;

        public int TrialDays { get; set; }

        public LimitPlan Limits { get; set; } = new();
    }
}
