using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfileDoctor;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IProfileDoctorService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> CreateAsync(CreateProfileDoctorRequest request);
        Task<ResponseApi<dynamic?>> UpdateAsync(UpdateProfileDoctorRequest request);
        Task<ResponseApi<dynamic>> DeleteAsync(DeleteDTO request);
        Task<ResponseApi<dynamic?>> ToggleBlockAsync(string id, string updatedBy);
    }
}
