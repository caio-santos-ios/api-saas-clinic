using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;
using MongoDB.Bson;

namespace api_clinic.src.Interfaces
{
    public interface IPlanRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Plan> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Plan> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<Plan?> GetByIdAsync(string id);
        Task<Plan?> CreateAsync(Plan entity);
        Task<Plan?> UpdateAsync(Plan entity);
        Task<Plan> DeleteAsync(Plan entity);
    }
}