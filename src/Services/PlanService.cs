using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Plan;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class PlanService(IPlanRepository repository) : IPlanService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Plan> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> plans = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(plans.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Planos listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> plan = await repository.GetByIdAggregateAsync(id);
                if (plan.Data is null) return new(null, 404, "Plano não encontrado");
                return new(plan.Data, 200, "Plano encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Plan?>> CreateAsync(CreatePlanRequest request)
        {
            try
            {
                Plan entity = new()
                {
                    Name = request.Name,
                    Type = request.Type,
                    Status = request.Status,
                    Cost = request.Cost,
                    Cycle = request.Cycle,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Plan? response = await repository.CreateAsync(entity);
                if (response is null) return new(null, 400, "Falha ao criar plano");

                return new(response, 201, "Plano criado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Plan?>> UpdateAsync(UpdatePlanRequest request)
        {
            try
            {
                Plan? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Plano não encontrado");

                if (!string.IsNullOrEmpty(request.Name)) existed.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Type)) existed.Type = request.Type;
                if (!string.IsNullOrEmpty(request.Status)) existed.Status = request.Status;
                if (request.Cost.HasValue) existed.Cost = request.Cost.Value;
                if (!string.IsNullOrEmpty(request.Cycle)) existed.Cycle = request.Cycle;
                if (request.Description is not null) existed.Description = request.Description;
                if (request.Active.HasValue) existed.Active = request.Active.Value;

                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = request.UpdatedBy;

                Plan? response = await repository.UpdateAsync(existed);
                if (response is null) return new(null, 400, "Falha ao atualizar plano");

                return new(response, 200, "Plano atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Plan>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Plan? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Plano não encontrado");

                existed.Deleted = true;
                existed.DeletedAt = DateTime.UtcNow;
                existed.DeletedBy = request.DeletedBy;

                Plan response = await repository.DeleteAsync(existed);
                return new(response, 200, "Plano removido com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
    }
}
