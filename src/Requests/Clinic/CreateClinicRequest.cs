using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.Clinic
{
    public class CreateClinicRequest : Request
    {
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome Fantasia é obrigatório.")]
        public string TradeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Razão Social é obrigatória.")]
        public string CorporateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string Phone { get; set; } = string.Empty;

        public AddressClinic Address { get; set; } = new();

        public SettingClinic Setting { get; set; } = new();

        public bool Active { get; set; } = true;
    }
}
