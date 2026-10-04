using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Appointment
{
    public class UpdateAppointmentStatusRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = string.Empty;

        public string? UpdatedBy { get; set; }
    }
}
