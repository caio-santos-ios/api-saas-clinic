using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Clinic;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class ClinicService(IClinicRepository repository) : IClinicService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Clinic> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> clinics = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(clinics.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Clínicas listadas com sucesso");
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
                dynamic? clinic = await repository.GetByIdAggregateAsync(id);
                if (clinic is null) return new(null, 404, "Clínica não encontrada");
                return new(clinic, 200, "Clínica encontrada");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Clinic?>> CreateAsync(CreateClinicRequest request)
        {
            try
            {
                Clinic? exists = await repository.GetByCNPJAsync(request.Cnpj);
                if (exists is not null) return new(null, 400, "Já existe uma clínica cadastrada com este CNPJ.");

                Clinic entity = new()
                {
                    Cnpj = request.Cnpj,
                    TradeName = request.TradeName,
                    CorporateName = request.CorporateName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address ?? new(),
                    Setting = request.Setting ?? new(),
                    Active = request.Active,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Clinic? created = await repository.CreateAsync(entity);
                if (created is null) return new(null, 400, "Falha ao cadastrar clínica");

                return new(created, 201, "Clínica criada com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Clinic?>> UpdateAsync(UpdateClinicRequest request)
        {
            try
            {
                Clinic? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Clínica não encontrada");

                if (!string.IsNullOrEmpty(request.Cnpj) && request.Cnpj != existed.Cnpj)
                {
                    Clinic? cnpjExists = await repository.GetByCNPJAsync(request.Cnpj);
                    if (cnpjExists is not null && cnpjExists.Id != existed.Id)
                    {
                        return new(null, 400, "Já existe outra clínica cadastrada com este CNPJ.");
                    }
                    existed.Cnpj = request.Cnpj;
                }

                if (!string.IsNullOrEmpty(request.TradeName)) existed.TradeName = request.TradeName;
                if (!string.IsNullOrEmpty(request.CorporateName)) existed.CorporateName = request.CorporateName;
                if (!string.IsNullOrEmpty(request.Email)) existed.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone)) existed.Phone = request.Phone;

                if (request.Address is not null)
                {
                    existed.Address = request.Address;
                }

                if (request.Setting is not null)
                {
                    existed.Setting = request.Setting;
                }

                if (request.Active.HasValue)
                {
                    existed.Active = request.Active.Value;
                }

                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = request.UpdatedBy;

                Clinic? updated = await repository.UpdateAsync(existed);
                if (updated is null) return new(null, 400, "Falha ao atualizar clínica");

                return new(updated, 200, "Clínica atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<Clinic?>> ToggleActiveAsync(string id, string updatedBy)
        {
            try
            {
                Clinic? existed = await repository.GetByIdAsync(id);
                if (existed is null) return new(null, 404, "Clínica não encontrada");

                existed.Active = !existed.Active;
                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = updatedBy;

                Clinic? updated = await repository.UpdateAsync(existed);
                if (updated is null) return new(null, 400, "Falha ao alterar status da clínica");

                return new(updated, 200, existed.Active ? "Clínica ativada com sucesso" : "Clínica desativada com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Clinic>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Clinic? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Clínica não encontrada");

                existed.Deleted = true;
                existed.DeletedBy = request.DeletedBy;
                existed.DeletedAt = DateTime.UtcNow;

                Clinic deleted = await repository.DeleteAsync(existed);
                return new(deleted, 200, "Clínica excluída com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
    }
}
