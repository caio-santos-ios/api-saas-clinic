using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Procedure
{
    public class CreateProcedureRequest : Request
    {
        [Required(ErrorMessage = "O Nome do procedimento é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "A duração deve ser entre 1 e 1440 minutos.")]
        public int DurationMinutes { get; set; } = 30;

        [Range(0, 9999999, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
        public decimal Price { get; set; } = 0;

        public bool Active { get; set; } = true;

        public string ClinicId { get; set; } = string.Empty;
    }
}
