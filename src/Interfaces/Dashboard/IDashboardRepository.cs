using api_clinic.src.Models.Base;

namespace api_clinic.src.Interfaces
{
    public interface IDashboardRepository
    {
        Task<ResponseApi<dynamic>> GetAllAsync(string userId, DateTime startDate, DateTime endDate);
    }
}