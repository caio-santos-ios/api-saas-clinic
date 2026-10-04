using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfileDoctor;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class ProfileDoctorService(
        IProfileDoctorRepository repository,
        IUserRepository userRepository
    ) : IProfileDoctorService
    {
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<User> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetAllAsync(pagination, clinicId);
                int count = await repository.GetCountDocumentsAsync(pagination, clinicId);
                PaginationApi<List<dynamic>> data = new(result.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Médicos listados com sucesso");
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
                return new(result.Data, 200, "Médicos listados com sucesso");
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
                dynamic? doctor = await repository.GetByIdAggregateAsync(id);
                if (doctor is null) return new(null, 404, "Médico não encontrado");
                return new(doctor, 200, "Médico encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> CreateAsync(CreateProfileDoctorRequest request)
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
                    ClinicId = request.ClinicId,
                    AccessProfile = "doctor",
                    ValidatedAccess = true,
                    Admin = false,
                    Master = false,
                    Photo = request.Photo ?? "",
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                ResponseApi<User?> userResp = await userRepository.CreateAsync(user);
                if (userResp.Data is null) return new(null, 400, "Falha ao criar médico");

                ProfileDoctor profile = new()
                {
                    UserId = user.Id,
                    Specialty = request.Specialty,
                    LicenseNumber = request.LicenseNumber,
                    LicenseState = request.LicenseState,
                    Bio = request.Bio,
                    Address = request.Address ?? new(),
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await repository.CreateAsync(profile);

                dynamic? created = await repository.GetByIdAggregateAsync(user.Id);
                return new(created, 201, "Médico cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> UpdateAsync(UpdateProfileDoctorRequest request)
        {
            try
            {
                ResponseApi<User?> userResp = await userRepository.GetByIdAsync(request.Id);
                User? user = userResp.Data;

                if (user is null)
                {
                    ProfileDoctor? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Médico não encontrado");

                if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
                if (!string.IsNullOrEmpty(request.Photo)) user.Photo = request.Photo;
                if (!string.IsNullOrEmpty(request.Password)) user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                if (request.Blocked.HasValue) user.Blocked = request.Blocked.Value;

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = request.UpdatedBy;
                await userRepository.UpdateAsync(user);

                ProfileDoctor? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is null)
                {
                    profile = new()
                    {
                        UserId = user.Id,
                        Specialty = request.Specialty ?? "",
                        LicenseNumber = request.LicenseNumber ?? "",
                        LicenseState = request.LicenseState ?? "",
                        Bio = request.Bio ?? "",
                        Address = request.Address ?? new(),
                        CreatedBy = request.UpdatedBy,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await repository.CreateAsync(profile);
                }
                else
                {
                    if (request.Specialty is not null) profile.Specialty = request.Specialty;
                    if (request.LicenseNumber is not null) profile.LicenseNumber = request.LicenseNumber;
                    if (request.LicenseState is not null) profile.LicenseState = request.LicenseState;
                    if (request.Bio is not null) profile.Bio = request.Bio;
                    if (request.Address is not null) profile.Address = request.Address;

                    profile.UpdatedAt = DateTime.UtcNow;
                    profile.UpdatedBy = request.UpdatedBy;
                    await repository.UpdateAsync(profile);
                }

                dynamic? updated = await repository.GetByIdAggregateAsync(user.Id);
                return new(updated, 200, "Médico atualizado com sucesso");
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
                    ProfileDoctor? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Médico não encontrado");

                user.Deleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = request.DeletedBy;
                await userRepository.UpdateAsync(user);

                ProfileDoctor? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is not null)
                {
                    profile.Deleted = true;
                    profile.DeletedAt = DateTime.UtcNow;
                    profile.DeletedBy = request.DeletedBy;
                    await repository.DeleteAsync(profile);
                }

                return new(new { id = user.Id }, 200, "Médico removido com sucesso");
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
                    ProfileDoctor? profileCheck = await repository.GetByIdAsync(id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Médico não encontrado");

                user.Blocked = !user.Blocked;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedBy;
                await userRepository.UpdateAsync(user);

                return new(new { id = user.Id, blocked = user.Blocked }, 200, user.Blocked ? "Médico bloqueado com sucesso" : "Médico desbloqueado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}
