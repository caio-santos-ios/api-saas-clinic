using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IProfileDoctorRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<User> pagination, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<User> pagination, string clinicId);
        Task<dynamic?> GetByIdAggregateAsync(string id);
        Task<ProfileDoctor?> GetByUserIdAsync(string userId);
        Task<ProfileDoctor?> GetByIdAsync(string id);
        Task<ProfileDoctor?> CreateAsync(ProfileDoctor entity);
        Task<ProfileDoctor?> UpdateAsync(ProfileDoctor entity);
        Task<ProfileDoctor> DeleteAsync(ProfileDoctor entity);
    }
}
