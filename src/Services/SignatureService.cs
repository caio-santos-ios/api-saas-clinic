using api_clinic.src.Handlers;
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
        IPlanRepository planRepository,
        AsaasHandler asaasHandler
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
                        {"status", "$status"},
                        {"paymentMethod", "$paymentMethod"},
                        {"clinicId", "$clinicId"},
                        {"planId", "$planId"},
                        {"cycle", "$cycle"},
                        {"value", "$value"},
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

        #region WEBHOOK
        public async Task<ResponseApi<dynamic?>> ProcessWebhookAsync(AsaasWebhookRequest request)
        {
            try
            {
                string? subscriptionId = request.Payment?.Subscription ?? request.Subscription?.Id;
                Signature? signature = null;

                if (!string.IsNullOrEmpty(subscriptionId))
                {
                    signature = await repository.GetByAsaasSubscriptionIdAsync(subscriptionId);
                }

                if (signature is null)
                {
                    string? customerId = request.Payment?.Customer ?? request.Subscription?.Customer;
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        signature = await repository.GetByAsaasCustomerIdAsync(customerId);
                    }
                }

                if (signature is null)
                {
                    return new(null, 404, "Assinatura não encontrada");
                }

                switch (request.Event)
                {
                    case "PAYMENT_RECEIVED":
                    case "PAYMENT_CONFIRMED":
                        signature.Status = "ATIVO";
                        if (request.Payment is not null && DateTime.TryParse(request.Payment.DueDate, out DateTime dueDate))
                        {
                            signature.NextDueDate = signature.Cycle == "yearly" ? dueDate.AddYears(1) : dueDate.AddMonths(1);
                            signature.EndDate = signature.NextDueDate;
                        }
                        break;

                    case "PAYMENT_OVERDUE":
                        signature.Status = "VENCIDO";
                        break;

                    case "SUBSCRIPTION_DELETED":
                    case "SUBSCRIPTION_INACTIVATED":
                    case "PAYMENT_DELETED":
                    case "PAYMENT_REFUNDED":
                        signature.Status = "CANCELADO";
                        break;

                    default:
                        break;
                }

                signature.UpdatedAt = DateTime.UtcNow;
                await repository.UpdateAsync(signature);

                return new(signature, 200, "Webhook processado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Erro ao processar webhook: {ex.Message}");
            }
        }
        #endregion

        #region SUBSCRIBE
        public async Task<ResponseApi<dynamic?>> SubscribeAsync(SubscribePlanRequest request)
        {
            try
            {
                Signature? signature = null;

                if (!string.IsNullOrEmpty(request.SignatureId))
                {
                    signature = await repository.GetByIdAsync(request.SignatureId);
                }

                if (signature is null && !string.IsNullOrEmpty(request.ClinicId))
                {
                    signature = await repository.GetByClinicIdAsync(request.ClinicId);
                }

                if (signature is null)
                {
                    return new(null, 404, "Assinatura não encontrada");
                }

                Clinic? clinic = await clinicRepository.GetByIdAsync(signature.ClinicId);
                if (clinic is null)
                {
                    return new(null, 404, "Clínica não encontrada");
                }

                string planId = !string.IsNullOrEmpty(request.PlanId) ? request.PlanId : signature.PlanId;
                Plan? plan = null;
                if (ObjectId.TryParse(planId, out _))
                {
                    plan = await planRepository.GetByIdAsync(planId);
                }

                if (string.IsNullOrEmpty(signature.AsaasCustomerId))
                {
                    var customer = await asaasHandler.GetOrCreateCustomerAsync(clinic.CorporateName, clinic.Cnpj, clinic.Email, clinic.Phone);
                    if (customer is not null)
                    {
                        signature.AsaasCustomerId = customer.Id;
                    }
                }

                if (string.IsNullOrEmpty(signature.AsaasCustomerId))
                {
                    return new(null, 400, "Falha ao vincular cliente no gateway de pagamento");
                }

                decimal value;
                if (plan is not null)
                {
                    value = plan.Cost;
                }
                else
                {
                    value = (request.PlanId?.ToLower(), request.Cycle) switch
                    {
                        ("bronze", "yearly") => 199m,
                        ("bronze", _) => 249m,
                        ("prata", "yearly") => 399m,
                        ("prata", _) => 499m,
                        ("ouro", "yearly") => 719m,
                        ("ouro", _) => 899m,
                        _ => 249m
                    };
                }

                string billingType = request.PaymentMethod.ToUpper() switch
                {
                    "BOLETO" => "BOLETO",
                    "CREDIT_CARD" or "CARTAO" => "CREDIT_CARD",
                    _ => "PIX"
                };

                string nextDueDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd");

                AsaasSubscriptionResponse? subResponse = await asaasHandler.CreateSubscriptionAsync(
                    signature.AsaasCustomerId,
                    value,
                    billingType,
                    nextDueDate,
                    request.CardData
                );

                if (subResponse is null || (subResponse.Errors is not null && subResponse.Errors.Count > 0))
                {
                    string errorMsg = subResponse?.Errors?.FirstOrDefault()?.Description ?? "Falha ao criar assinatura no gateway de pagamento";
                    return new(null, 400, errorMsg);
                }

                signature.AsaasSubscriptionId = subResponse.Id;
                if (plan is not null) signature.PlanId = plan.Id;
                else if (!string.IsNullOrEmpty(request.PlanId)) signature.PlanId = request.PlanId;
                signature.Cycle = request.Cycle;
                signature.PaymentMethod = request.PaymentMethod;
                signature.Value = value;
                signature.Status = "PENDENTE";
                signature.UpdatedAt = DateTime.UtcNow;
                await repository.UpdateAsync(signature);

                if (billingType == "PIX")
                {
                    var payment = await asaasHandler.GetLastPaymentFromSubscriptionAsync(subResponse.Id);
                    if (payment is null)
                    {
                        await Task.Delay(1000);
                        payment = await asaasHandler.GetLastPaymentFromSubscriptionAsync(subResponse.Id);
                    }

                    AsaasPixResponse? pix = payment is not null ? await asaasHandler.GetPixQrCodeAsync(payment.Id) : null;

                    return new(new
                    {
                        signatureId = signature.Id,
                        asaasSubscriptionId = subResponse.Id,
                        paymentId = payment?.Id,
                        pixQrCode = pix?.EncodedImage,
                        pixCopyPaste = pix?.Payload,
                        expirationDate = pix?.ExpirationDate
                    }, 200, "Assinatura Pix gerada com sucesso");
                }

                if (billingType == "BOLETO")
                {
                    var payment = await asaasHandler.GetLastPaymentFromSubscriptionAsync(subResponse.Id);
                    if (payment is null)
                    {
                        await Task.Delay(1000);
                        payment = await asaasHandler.GetLastPaymentFromSubscriptionAsync(subResponse.Id);
                    }

                    AsaasBoletoResponse? boleto = payment is not null ? await asaasHandler.GetBoletoIdentificationFieldAsync(payment.Id) : null;

                    return new(new
                    {
                        signatureId = signature.Id,
                        asaasSubscriptionId = subResponse.Id,
                        paymentId = payment?.Id,
                        identificationField = boleto?.IdentificationField,
                        barCode = boleto?.BarCode,
                        bankSlipUrl = payment?.BankSlipUrl
                    }, 200, "Boleto gerado com sucesso");
                }

                return new(new
                {
                    signatureId = signature.Id,
                    asaasSubscriptionId = subResponse.Id,
                    status = subResponse.Status
                }, 200, "Assinatura processada com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro ao processar a assinatura: {ex.Message}");
            }
        }
        #endregion
    }
}
