using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.ProfilePatient
{
    public class CreateProfilePatientRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        public string Phone { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string? Photo { get; set; }

        public string? Cpf { get; set; }

        public string? Rg { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public string? BloodType { get; set; }

        public List<string>? Allergies { get; set; }

        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? EmergencyContactRelationship { get; set; }

        public Address? Address { get; set; }

        public string? Notes { get; set; }

        public string? ClinicId { get; set; }

        public string? CreatedBy { get; set; }
    }
}
