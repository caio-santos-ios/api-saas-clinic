using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;
using MongoDB.Bson;

namespace api_clinic.src.Interfaces
{
    public interface IClinicRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Clinic> pagination);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Clinic> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Clinic> pagination);
        Task<dynamic?> GetByIdAggregateAsync(string id);
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