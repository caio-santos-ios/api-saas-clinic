using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Appointment
{
    public class UpdateAppointmentRequest
    {
        [Required(ErrorMessage = "Id é obrigatório")]
        public string Id { get; set; } = string.Empty;

        public string? PatientId { get; set; }

        public string? DoctorId { get; set; }

        public string? ProcedureId { get; set; }

        public DateTime? Date { get; set; }

        public string? StartTime { get; set; }

        public string? EndTime { get; set; }

        public string? Status { get; set; }

        public string? Notes { get; set; }

        public decimal? Price { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
