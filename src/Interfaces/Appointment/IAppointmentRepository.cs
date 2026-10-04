using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> CreateAsync(Appointment entity);
        Task<Appointment?> UpdateAsync(Appointment entity);
        Task<Appointment> DeleteAsync(Appointment entity);
        Task<Appointment?> GetByIdAsync(string id);
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Appointment> pagination, string clinicId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Appointment> pagination, string clinicId);
        Task<dynamic?> GetByIdAggregateAsync(string id);
    }
}
