using api_clinic.src.Models;

namespace api_clinic.src.Requests.ProfileDoctor
{
    public class UpdateProfileDoctorRequest
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? Photo { get; set; }
        public bool? Blocked { get; set; }
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseState { get; set; }
        public string? Bio { get; set; }
        public Address? Address { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
