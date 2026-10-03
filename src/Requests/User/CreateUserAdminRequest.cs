using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.User
{
    public class CreateUserAdminRequest
    {
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome fantasia é obrigatório.")]
        public string TradeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Razão social é obrigatória.")]
        public string CorporateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        public string Password { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public AddressClinic Address { get; set; } = new();
        public SettingClinic Setting { get; set; } = new();
    }
}