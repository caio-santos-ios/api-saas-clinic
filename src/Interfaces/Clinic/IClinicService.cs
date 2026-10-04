using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Clinic;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IClinicService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Clinic?>> CreateAsync(CreateClinicRequest request);
        Task<ResponseApi<Clinic?>> UpdateAsync(UpdateClinicRequest request);
        Task<ResponseApi<Clinic>> DeleteAsync(DeleteDTO request);
        Task<ResponseApi<Clinic?>> ToggleActiveAsync(string id, string updatedBy);
    }
}
