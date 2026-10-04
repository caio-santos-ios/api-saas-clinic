using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfilePatient;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class ProfilePatientService(
        IProfilePatientRepository repository,
        IUserRepository userRepository
    ) : IProfilePatientService
    {
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<User> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetAllAsync(pagination, clinicId);
                int count = await repository.GetCountDocumentsAsync(pagination, clinicId);
                PaginationApi<List<dynamic>> data = new(result.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Pacientes listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(string clinicId)
        {
            try
            {
                ResponseApi<List<dynamic>> result = await repository.GetSelectAsync(clinicId);
                return new(result.Data, 200, "Pacientes listados com sucesso");
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
                dynamic? patient = await repository.GetByIdAggregateAsync(id);
                if (patient is null) return new(null, 404, "Paciente não encontrado");
                return new(patient, 200, "Paciente encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> CreateAsync(CreateProfilePatientRequest request)
        {
            try
            {
                ResponseApi<User?> existed = await userRepository.GetByEmailAsync(request.Email);
                if (existed.Data is not null) return new(null, 400, "E-mail já cadastrado");

                User user = new()
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = BCrypt.Net.BCrypt.HashPassword(string.IsNullOrEmpty(request.Password) ? "123456" : request.Password),
                    ClinicId = request.ClinicId ?? "",
                    AccessProfile = "patient",
                    ValidatedAccess = true,
                    Admin = false,
                    Master = false,
                    Photo = request.Photo ?? "",
                    CreatedBy = request.CreatedBy ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                ResponseApi<User?> userResp = await userRepository.CreateAsync(user);
                if (userResp.Data is null) return new(null, 400, "Falha ao criar paciente");

                ProfilePatient profile = new()
                {
                    UserId = user.Id,
                    Cpf = request.Cpf ?? "",
                    Rg = request.Rg ?? "",
                    BirthDate = request.BirthDate,
                    Gender = request.Gender ?? "",
                    BloodType = request.BloodType ?? "",
                    Allergies = request.Allergies ?? [],
                    EmergencyContactName = request.EmergencyContactName ?? "",
                    EmergencyContactPhone = request.EmergencyContactPhone ?? "",
                    EmergencyContactRelationship = request.EmergencyContactRelationship ?? "",
                    Address = request.Address ?? new(),
                    Notes = request.Notes ?? "",
                    CreatedBy = request.CreatedBy ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await repository.CreateAsync(profile);

                dynamic? created = await repository.GetByIdAggregateAsync(user.Id);
                return new(created, 201, "Paciente cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> UpdateAsync(UpdateProfilePatientRequest request)
        {
            try
            {
                ResponseApi<User?> userResp = await userRepository.GetByIdAsync(request.Id);
                User? user = userResp.Data;

                if (user is null)
                {
                    ProfilePatient? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Paciente não encontrado");

                if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
                if (!string.IsNullOrEmpty(request.Photo)) user.Photo = request.Photo;
                if (!string.IsNullOrEmpty(request.Password)) user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                if (request.Blocked.HasValue) user.Blocked = request.Blocked.Value;

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = request.UpdatedBy ?? "";
                await userRepository.UpdateAsync(user);

                ProfilePatient? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is null)
                {
                    profile = new()
                    {
                        UserId = user.Id,
                        Cpf = request.Cpf ?? "",
                        Rg = request.Rg ?? "",
                        BirthDate = request.BirthDate,
                        Gender = request.Gender ?? "",
                        BloodType = request.BloodType ?? "",
                        Allergies = request.Allergies ?? [],
                        EmergencyContactName = request.EmergencyContactName ?? "",
                        EmergencyContactPhone = request.EmergencyContactPhone ?? "",
                        EmergencyContactRelationship = request.EmergencyContactRelationship ?? "",
                        Address = request.Address ?? new(),
                        Notes = request.Notes ?? "",
                        CreatedBy = request.UpdatedBy ?? "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await repository.CreateAsync(profile);
                }
                else
                {
                    if (request.Cpf is not null) profile.Cpf = request.Cpf;
                    if (request.Rg is not null) profile.Rg = request.Rg;
                    if (request.BirthDate.HasValue) profile.BirthDate = request.BirthDate;
                    if (request.Gender is not null) profile.Gender = request.Gender;
                    if (request.BloodType is not null) profile.BloodType = request.BloodType;
                    if (request.Allergies is not null) profile.Allergies = request.Allergies;
                    if (request.EmergencyContactName is not null) profile.EmergencyContactName = request.EmergencyContactName;
                    if (request.EmergencyContactPhone is not null) profile.EmergencyContactPhone = request.EmergencyContactPhone;
                    if (request.EmergencyContactRelationship is not null) profile.EmergencyContactRelationship = request.EmergencyContactRelationship;
                    if (request.Address is not null) profile.Address = request.Address;
                    if (request.Notes is not null) profile.Notes = request.Notes;

                    profile.UpdatedAt = DateTime.UtcNow;
                    profile.UpdatedBy = request.UpdatedBy ?? "";
                    await repository.UpdateAsync(profile);
                }

                dynamic? updated = await repository.GetByIdAggregateAsync(user.Id);
                return new(updated, 200, "Paciente atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<User?> userResp = await userRepository.GetByIdAsync(request.Id);
                User? user = userResp.Data;

                if (user is null)
                {
                    ProfilePatient? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Paciente não encontrado");

                user.Deleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = request.DeletedBy;
                await userRepository.UpdateAsync(user);

                ProfilePatient? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is not null)
                {
                    profile.Deleted = true;
                    profile.DeletedAt = DateTime.UtcNow;
                    profile.DeletedBy = request.DeletedBy;
                    await repository.DeleteAsync(profile);
                }

                return new(new { id = user.Id }, 200, "Paciente removido com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> ToggleBlockAsync(string id, string updatedBy)
        {
            try
            {
                ResponseApi<User?> userResp = await userRepository.GetByIdAsync(id);
                User? user = userResp.Data;

                if (user is null)
                {
                    ProfilePatient? profileCheck = await repository.GetByIdAsync(id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Paciente não encontrado");

                user.Blocked = !user.Blocked;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedBy;
                await userRepository.UpdateAsync(user);

                return new(new { id = user.Id, blocked = user.Blocked }, 200, user.Blocked ? "Paciente bloqueado com sucesso" : "Paciente desbloqueado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}
