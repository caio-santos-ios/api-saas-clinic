using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Appointment;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class AppointmentService(
        IAppointmentRepository repository,
        IProcedureRepository procedureRepository
    ) : IAppointmentService
    {
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<Appointment> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetAllAsync(pagination, clinicId);
                int count = await repository.GetCountDocumentsAsync(pagination, clinicId);
                PaginationApi<List<dynamic>> data = new(result.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Agendamentos listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                dynamic? appointment = await repository.GetByIdAggregateAsync(id);
                if (appointment is null) return new(null, 404, "Agendamento não encontrado");
                return new(appointment, 200, "Agendamento encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> CreateAsync(CreateAppointmentRequest request)
        {
            try
            {
                decimal price = request.Price;
                string endTime = request.EndTime ?? "";

                if (!string.IsNullOrEmpty(request.ProcedureId))
                {
                    Procedure? proc = await procedureRepository.GetByIdAsync(request.ProcedureId);
                    if (proc != null)
                    {
                        if (price <= 0) price = proc.Price;
                        if (string.IsNullOrEmpty(endTime) && !string.IsNullOrEmpty(request.StartTime))
                        {
                            if (TimeSpan.TryParse(request.StartTime, out TimeSpan start))
                            {
                                TimeSpan calculatedEnd = start.Add(TimeSpan.FromMinutes(proc.DurationMinutes > 0 ? proc.DurationMinutes : 30));
                                endTime = calculatedEnd.ToString(@"hh\:mm");
                            }
                        }
                    }
                }

                Appointment appointment = new()
                {
                    ClinicId = request.ClinicId ?? "",
                    PatientId = request.PatientId,
                    DoctorId = request.DoctorId,
                    ProcedureId = request.ProcedureId,
                    Date = request.Date,
                    StartTime = request.StartTime,
                    EndTime = string.IsNullOrEmpty(endTime) ? request.StartTime : endTime,
                    Status = string.IsNullOrEmpty(request.Status) ? "AGENDADO" : request.Status.ToUpper(),
                    Notes = request.Notes ?? "",
                    Price = price,
                    CreatedBy = request.CreatedBy ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await repository.CreateAsync(appointment);

                dynamic? created = await repository.GetByIdAggregateAsync(appointment.Id);
                return new(created, 201, "Agendamento cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> UpdateAsync(UpdateAppointmentRequest request)
        {
            try
            {
                Appointment? appointment = await repository.GetByIdAsync(request.Id);
                if (appointment is null) return new(null, 404, "Agendamento não encontrado");

                if (!string.IsNullOrEmpty(request.PatientId)) appointment.PatientId = request.PatientId;
                if (!string.IsNullOrEmpty(request.DoctorId)) appointment.DoctorId = request.DoctorId;
                if (!string.IsNullOrEmpty(request.ProcedureId)) appointment.ProcedureId = request.ProcedureId;
                if (request.Date.HasValue) appointment.Date = request.Date.Value;
                if (!string.IsNullOrEmpty(request.StartTime)) appointment.StartTime = request.StartTime;
                if (!string.IsNullOrEmpty(request.EndTime)) appointment.EndTime = request.EndTime;
                if (!string.IsNullOrEmpty(request.Status)) appointment.Status = request.Status.ToUpper();
                if (request.Notes is not null) appointment.Notes = request.Notes;
                if (request.Price.HasValue) appointment.Price = request.Price.Value;

                appointment.UpdatedAt = DateTime.UtcNow;
                appointment.UpdatedBy = request.UpdatedBy ?? "";

                await repository.UpdateAsync(appointment);

                dynamic? updated = await repository.GetByIdAggregateAsync(appointment.Id);
                return new(updated, 200, "Agendamento atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> UpdateStatusAsync(UpdateAppointmentStatusRequest request)
        {
            try
            {
                Appointment? appointment = await repository.GetByIdAsync(request.Id);
                if (appointment is null) return new(null, 404, "Agendamento não encontrado");

                appointment.Status = request.Status.ToUpper();
                appointment.UpdatedAt = DateTime.UtcNow;
                appointment.UpdatedBy = request.UpdatedBy ?? "";

                await repository.UpdateAsync(appointment);

                dynamic? updated = await repository.GetByIdAggregateAsync(appointment.Id);
                return new(updated, 200, "Status do agendamento atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Appointment? appointment = await repository.GetByIdAsync(request.Id);
                if (appointment is null) return new(null, 404, "Agendamento não encontrado");

                appointment.Deleted = true;
                appointment.DeletedAt = DateTime.UtcNow;
                appointment.DeletedBy = request.DeletedBy;
                await repository.DeleteAsync(appointment);

                return new(new { id = appointment.Id }, 200, "Agendamento cancelado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}
