using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests
{
    public class LoginRequest : Request
    {
        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Order = 1)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatório.")]
        [Display(Order = 2)]
        public string Password { get; set; } = string.Empty;
        public UserDevice Device { get; set; } = new();
    }
}