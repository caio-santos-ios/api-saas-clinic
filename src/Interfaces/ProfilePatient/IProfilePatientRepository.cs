using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IProfilePatientRepository
    {
        Task<ProfilePatient?> CreateAsync(ProfilePatient entity);
        Task<ProfilePatient?> UpdateAsync(ProfilePatient entity);
        Task<ProfilePatient> DeleteAsync(ProfilePatient entity);
        Task<ProfilePatient?> GetByIdAsync(string id);
        Task<ProfilePatient?> GetByUserIdAsync(string userId);
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<User> pagination, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<User> pagination, string clinicId);
        Task<dynamic?> GetByIdAggregateAsync(string id);
    }
}
