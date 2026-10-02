using api_clinic.src.Models;
using MongoDB.Bson;

namespace api_clinic.src.Interfaces
{
    public interface IPlanRepository
    {
        Task<List<dynamic>> GetAllAsync(List<BsonDocument> pipeline);
        Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline);
        Task<Plan?> GetByIdAsync(string id);
        Task<int> GetCountDocumentsAsync(List<BsonDocument> pipeline);
        Task<Plan?> CreateAsync(Plan entity);
        Task<Plan?> UpdateAsync(Plan entity);
        Task<Plan> DeleteAsync(Plan entity);
    }
}