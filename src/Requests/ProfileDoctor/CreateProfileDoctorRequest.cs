using api_clinic.src.Models;

namespace api_clinic.src.Requests.ProfileDoctor
{
    public class CreateProfileDoctorRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseState { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public Address Address { get; set; } = new();
        public string ClinicId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
