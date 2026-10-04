using api_clinic.src.Models;
using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.ProfileEmployee
{
    public class CreateProfileEmployeeRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        public string Phone { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O cargo / função é obrigatório.")]
        public string Role { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime? HireDate { get; set; }
        public Address Address { get; set; } = new();
        public string ClinicId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
