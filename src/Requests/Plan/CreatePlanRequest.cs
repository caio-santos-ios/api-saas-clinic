using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Plan
{
    public class CreatePlanRequest : Request
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Tipo é obrigatório.")]
        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = "ATIVO";

        [Required(ErrorMessage = "O Valor é obrigatório.")]
        public decimal Cost { get; set; } = 0;

        public string Cycle { get; set; } = "monthly";

        public string Description { get; set; } = string.Empty;
    }
}
