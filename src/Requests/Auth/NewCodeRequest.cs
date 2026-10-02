using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests
{
    public class NewCodeRequest : Request
    {
        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        public string Email { get; set; } = string.Empty;
    }
}