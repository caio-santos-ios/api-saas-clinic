using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IProfileEmployeeRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<User> pagination, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<User> pagination, string clinicId);
        Task<dynamic?> GetByIdAggregateAsync(string id);
        Task<ProfileEmployee?> GetByUserIdAsync(string userId);
        Task<ProfileEmployee?> GetByIdAsync(string id);
        Task<ProfileEmployee?> CreateAsync(ProfileEmployee entity);
        Task<ProfileEmployee?> UpdateAsync(ProfileEmployee entity);
        Task<ProfileEmployee> DeleteAsync(ProfileEmployee entity);
    }
}
