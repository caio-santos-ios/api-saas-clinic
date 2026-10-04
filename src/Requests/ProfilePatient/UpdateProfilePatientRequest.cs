using System.ComponentModel.DataAnnotations;
using api_clinic.src.Models;

namespace api_clinic.src.Requests.ProfilePatient
{
    public class UpdateProfilePatientRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public string Id { get; set; } = string.Empty;

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? Photo { get; set; }

        public bool? Blocked { get; set; }

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

        public string? UpdatedBy { get; set; }
    }
}
