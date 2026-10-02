using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IAttachmentService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Attachment?>> CreateAsync(CreateAttachmentRequest request);
        Task<ResponseApi<Attachment?>> UpdateAsync(UpdateAttachmentRequest request);
        Task<ResponseApi<Attachment>> DeleteAsync(DeleteDTO request);
    }
}