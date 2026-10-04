using api_clinic.src.Models;
using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.ProfileEmployee
{
    public class UpdateProfileEmployeeRequest
    {
        [Required(ErrorMessage = "O Id é obrigatório.")]
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? Photo { get; set; }
        public bool? Blocked { get; set; }
        public string? Role { get; set; }
        public string? Cpf { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public Address? Address { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
