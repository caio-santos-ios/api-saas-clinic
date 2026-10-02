using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Attachment> pagination);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Attachment> pagination);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<Attachment?>> GetByIdAsync(string id);
        Task<List<Attachment>> GetByParentIdAsync(string parentId, string parent);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Attachment> pagination);
        Task<ResponseApi<Attachment?>> CreateAsync(Attachment entity);
        Task<ResponseApi<Attachment?>> UpdateAsync(Attachment request);
        Task<ResponseApi<Attachment>> DeleteAsync(DeleteDTO request);
    }
}