using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Signature;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface ISignatureService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<dynamic?>> GetByClinicIdAsync(string clinicId);
        Task<ResponseApi<Signature?>> CreateAsync(CreateSignatureRequest request);
        Task<ResponseApi<Signature?>> UpdateAsync(UpdateSignatureRequest request);
        Task<ResponseApi<Signature>> DeleteAsync(DeleteDTO request);
        Task<ResponseApi<dynamic?>> ProcessWebhookAsync(AsaasWebhookRequest request);
        Task<ResponseApi<dynamic?>> SubscribeAsync(SubscribePlanRequest request);
    }
}
