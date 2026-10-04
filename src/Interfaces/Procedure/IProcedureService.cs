using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Procedure;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IProcedureService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request, string clinicId);
        Task<ResponseApi<Procedure?>> GetByIdAsync(string id);
        Task<ResponseApi<Procedure?>> CreateAsync(CreateProcedureRequest request);
        Task<ResponseApi<Procedure?>> UpdateAsync(UpdateProcedureRequest request);
        Task<ResponseApi<Procedure?>> ToggleActiveAsync(string id, string updatedBy);
        Task<ResponseApi<Procedure>> DeleteAsync(DeleteDTO request);
    }
}
