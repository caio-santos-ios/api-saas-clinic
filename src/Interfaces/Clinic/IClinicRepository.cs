using api_clinic.src.Models;
using MongoDB.Bson;

namespace api_clinic.src.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<dynamic>> GetAllAsync(List<BsonDocument> pipeline);
        Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline);
        Task<Clinic?> GetByIdAsync(string id);
        Task<Clinic?> GetByCNPJAsync(string cnpj);
        Task<int> GetCountDocumentsAsync(List<BsonDocument> pipeline);
        Task<Clinic?> CreateAsync(Clinic entity);
        Task<Clinic?> UpdateAsync(Clinic entity);
        Task<Clinic> DeleteAsync(Clinic entity);
    }
}