using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Procedure;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class ProcedureService(IProcedureRepository repository) : IProcedureService
    {
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<Procedure> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetAllAsync(pagination, clinicId);
                int count = await repository.GetCountDocumentsAsync(pagination, clinicId);
                PaginationApi<List<dynamic>> data = new(result.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Procedimentos listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<Procedure> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetSelectAsync(pagination, clinicId);
                return new(result.Data, 200, "Procedimentos listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Procedure?>> GetByIdAsync(string id)
        {
            try
            {
                Procedure? entity = await repository.GetByIdAsync(id);
                if (entity is null) return new(null, 404, "Procedimento não encontrado");
                return new(entity, 200, "Procedimento encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Procedure?>> CreateAsync(CreateProcedureRequest request)
        {
            try
            {
                Procedure entity = new()
                {
                    ClinicId = request.ClinicId,
                    Name = request.Name,
                    Description = request.Description,
                    Code = request.Code,
                    DurationMinutes = request.DurationMinutes,
                    Price = request.Price,
                    Active = request.Active,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Procedure? created = await repository.CreateAsync(entity);
                if (created is null) return new(null, 400, "Falha ao cadastrar procedimento");

                return new(created, 201, "Procedimento cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Procedure?>> UpdateAsync(UpdateProcedureRequest request)
        {
            try
            {
                Procedure? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Procedimento não encontrado");

                if (!string.IsNullOrEmpty(request.Name)) existed.Name = request.Name;
                existed.Description = request.Description ?? existed.Description;
                existed.Code = request.Code ?? existed.Code;
                existed.DurationMinutes = request.DurationMinutes > 0 ? request.DurationMinutes : existed.DurationMinutes;
                existed.Price = request.Price >= 0 ? request.Price : existed.Price;

                if (request.Active.HasValue)
                {
                    existed.Active = request.Active.Value;
                }

                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = request.UpdatedBy;

                Procedure? updated = await repository.UpdateAsync(existed);
                if (updated is null) return new(null, 400, "Falha ao atualizar procedimento");

                return new(updated, 200, "Procedimento atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Procedure?>> ToggleActiveAsync(string id, string updatedBy)
        {
            try
            {
                Procedure? existed = await repository.GetByIdAsync(id);
                if (existed is null) return new(null, 404, "Procedimento não encontrado");

                existed.Active = !existed.Active;
                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = updatedBy;

                Procedure? updated = await repository.UpdateAsync(existed);
                if (updated is null) return new(null, 400, "Falha ao alterar status do procedimento");

                string statusMsg = existed.Active ? "Procedimento ATIVO com sucesso" : "Procedimento INATIVO com sucesso";
                return new(updated, 200, statusMsg);
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Procedure>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Procedure? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Procedimento não encontrado");

                existed.Deleted = true;
                existed.DeletedBy = request.DeletedBy;
                existed.DeletedAt = DateTime.UtcNow;

                Procedure deleted = await repository.DeleteAsync(existed);
                return new(deleted, 200, "Procedimento excluído com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    }
}
