using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Interfaces
{
    public interface IProcedureRepository
    {
        Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Procedure> pagination, string clinicId);
        Task<ResponseApi<List<dynamic>>> GetSelectAsync(PaginationUtil<Procedure> pagination, string clinicId);
        Task<int> GetCountDocumentsAsync(PaginationUtil<Procedure> pagination, string clinicId);
        Task<Procedure?> GetByIdAsync(string id);
        Task<Procedure?> CreateAsync(Procedure entity);
        Task<Procedure?> UpdateAsync(Procedure entity);
        Task<Procedure> DeleteAsync(Procedure entity);
    }
}
