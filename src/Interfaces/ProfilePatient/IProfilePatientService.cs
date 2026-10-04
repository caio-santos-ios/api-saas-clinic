using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfilePatient;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IProfilePatientService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> CreateAsync(CreateProfilePatientRequest request);
        Task<ResponseApi<dynamic?>> UpdateAsync(UpdateProfilePatientRequest request);
        Task<ResponseApi<dynamic>> DeleteAsync(DeleteDTO request);
        Task<ResponseApi<dynamic?>> ToggleBlockAsync(string id, string updatedBy);
    }
}
