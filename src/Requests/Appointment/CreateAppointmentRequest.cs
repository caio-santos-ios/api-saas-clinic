using System.ComponentModel.DataAnnotations;

namespace api_clinic.src.Requests.Appointment
{
    public class CreateAppointmentRequest
    {
        [Required(ErrorMessage = "Paciente é obrigatório")]
        public string PatientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Médico é obrigatório")]
        public string DoctorId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Procedimento é obrigatório")]
        public string ProcedureId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Horário de início é obrigatório")]
        public string StartTime { get; set; } = string.Empty;

        public string? EndTime { get; set; }

        public string Status { get; set; } = "AGENDADO";

        public string? Notes { get; set; }

        public decimal Price { get; set; } = 0;

        public string? ClinicId { get; set; }

        public string? CreatedBy { get; set; }
    }
}
