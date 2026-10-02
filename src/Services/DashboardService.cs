using api_clinic.src.Interfaces;
using api_clinic.src.Models.Base;

namespace api_clinic.src.Services
{
    public class DashboardService(
        IDashboardRepository repository
    ) : IDashboardService
    {
        public async Task<ResponseApi<dynamic>> GetAllAsync(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                ResponseApi<dynamic> data = await repository.GetAllAsync(userId, startDate, endDate);

                return new(data.Data, 200, "Dashboard listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    }
}