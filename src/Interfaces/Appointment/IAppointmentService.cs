using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Appointment;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IAppointmentService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> CreateAsync(CreateAppointmentRequest request);
        Task<ResponseApi<dynamic?>> UpdateAsync(UpdateAppointmentRequest request);
        Task<ResponseApi<dynamic?>> UpdateStatusAsync(UpdateAppointmentStatusRequest request);
        Task<ResponseApi<dynamic>> DeleteAsync(DeleteDTO request);
    }
}
