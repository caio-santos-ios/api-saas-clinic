using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.Signature;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;
using MongoDB.Bson;

namespace api_clinic.src.Services
{
    public class SignatureService(
        ISignatureRepository repository,
        IClinicRepository clinicRepository,
        IPlanRepository planRepository
    ) : ISignatureService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Signature> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> signatures = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(signatures.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Assinaturas listadas com sucesso");
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
                List<BsonDocument> pipeline = [
                    new("$match", new BsonDocument
                    {
                        { "_id", new ObjectId(id) },
                        { "deleted", false }
                    }),

                    new("$addFields", new BsonDocument
                    {
                        {"clinicObjId", new BsonDocument("$toObjectId", "$clinicId")},
                    }),

                    MongoUtil.Lookup("clinics", ["$clinicObjId"], ["$_id"], "_clinics", [["deleted", false]], 1),

                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", MongoUtil.ToString("$_id")},
                        {"clinicSetting", MongoUtil.First("_clinics.setting")},
                    })
                ];

                dynamic? signature = await repository.GetByIdAggregateAsync(pipeline);
                
                if (signature is null) return new(null, 404, "Assinatura não encontrada");
                return new(signature, 200, "Assinatura encontrada");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByClinicIdAsync(string clinicId)
        {
            try
            {
                Signature? signature = await repository.GetByClinicIdAsync(clinicId);
                if (signature is null) return new(null, 404, "Assinatura da clínica não encontrada");
                return new();
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Signature?>> CreateAsync(CreateSignatureRequest request)
        {
            try
            {
                Clinic? clinic = await clinicRepository.GetByIdAsync(request.ClinicId);
                if (clinic is null) return new(null, 404, "Clínica não encontrada");

                Plan? plan = await planRepository.GetByIdAsync(request.PlanId);
                if (plan is null) return new(null, 404, "Plano não encontrado");

                Signature entity = new()
                {
                    ClinicId = request.ClinicId,
                    PlanId = request.PlanId,
                    Status = request.Status,
                    PaymentMethod = request.PaymentMethod,
                    Cycle = request.Cycle,
                    Value = request.Value > 0 ? request.Value : plan.Cost,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    NextDueDate = request.NextDueDate ?? request.StartDate.AddMonths(request.Cycle == "yearly" ? 12 : 1),
                    AsaasSubscriptionId = request.AsaasSubscriptionId,
                    AsaasCustomerId = request.AsaasCustomerId,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Signature? response = await repository.CreateAsync(entity);
                if (response is null) return new(null, 400, "Falha ao criar assinatura");

                return new(response, 201, "Assinatura criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Signature?>> UpdateAsync(UpdateSignatureRequest request)
        {
            try
            {
                Signature? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Assinatura não encontrada");

                if (!string.IsNullOrEmpty(request.PlanId))
                {
                    Plan? plan = await planRepository.GetByIdAsync(request.PlanId);
                    if (plan is null) return new(null, 404, "Plano não encontrado");
                    existed.PlanId = request.PlanId;
                }

                if (!string.IsNullOrEmpty(request.Status)) existed.Status = request.Status;
                if (!string.IsNullOrEmpty(request.PaymentMethod)) existed.PaymentMethod = request.PaymentMethod;
                if (!string.IsNullOrEmpty(request.Cycle)) existed.Cycle = request.Cycle;
                if (request.Value.HasValue) existed.Value = request.Value.Value;
                if (request.EndDate.HasValue) existed.EndDate = request.EndDate.Value;
                if (request.NextDueDate.HasValue) existed.NextDueDate = request.NextDueDate.Value;
                if (!string.IsNullOrEmpty(request.AsaasSubscriptionId)) existed.AsaasSubscriptionId = request.AsaasSubscriptionId;
                if (!string.IsNullOrEmpty(request.AsaasCustomerId)) existed.AsaasCustomerId = request.AsaasCustomerId;
                if (request.Active.HasValue) existed.Active = request.Active.Value;

                existed.UpdatedAt = DateTime.UtcNow;
                existed.UpdatedBy = request.UpdatedBy;

                Signature? response = await repository.UpdateAsync(existed);
                if (response is null) return new(null, 400, "Falha ao atualizar assinatura");

                return new(response, 200, "Assinatura atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Signature>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                Signature? existed = await repository.GetByIdAsync(request.Id);
                if (existed is null) return new(null, 404, "Assinatura não encontrada");

                existed.Deleted = true;
                existed.DeletedAt = DateTime.UtcNow;
                existed.DeletedBy = request.DeletedBy;

                Signature response = await repository.DeleteAsync(existed);
                return new(response, 200, "Assinatura removida com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
    }
}
