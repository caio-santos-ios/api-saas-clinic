using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Plan;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IPlanService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Plan?>> CreateAsync(CreatePlanRequest request);
        Task<ResponseApi<Plan?>> UpdateAsync(UpdatePlanRequest request);
        Task<ResponseApi<Plan>> DeleteAsync(DeleteDTO request);
    }
}
