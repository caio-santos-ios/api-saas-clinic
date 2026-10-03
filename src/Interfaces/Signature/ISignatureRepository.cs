using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;
using MongoDB.Bson;

namespace api_clinic.src.Interfaces
{
    public interface ISignatureRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Signature> pagination);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Signature> pagination);
        Task<dynamic?> GetByIdAggregateAsync(List<BsonDocument> pipeline);
        Task<Signature?> GetByIdAsync(string id);
        Task<Signature?> GetByClinicIdAsync(string clinicId);
        Task<Signature?> GetByAsaasSubscriptionIdAsync(string asaasSubscriptionId);
        Task<Signature?> GetByAsaasCustomerIdAsync(string asaasCustomerId);
        Task<Signature?> CreateAsync(Signature entity);
        Task<Signature?> UpdateAsync(Signature entity);
        Task<Signature> DeleteAsync(Signature entity);
    }
}
