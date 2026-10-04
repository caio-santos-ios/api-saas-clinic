using api_clinic.src.Interfaces;
using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests.ProfileEmployee;
using api_clinic.src.Shared.DTOs;
using api_clinic.src.Shared.Utils;

namespace api_clinic.src.Services
{
    public class ProfileEmployeeService(
        IProfileEmployeeRepository repository,
        IUserRepository userRepository
    ) : IProfileEmployeeService
    {
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request, string clinicId)
        {
            try
            {
                PaginationUtil<User> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> result = await repository.GetAllAsync(pagination, clinicId);
                int count = await repository.GetCountDocumentsAsync(pagination, clinicId);
                PaginationApi<List<dynamic>> data = new(result.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Funcionários listados com sucesso");
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
                return new(result.Data, 200, "Funcionários listados com sucesso");
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
                dynamic? employee = await repository.GetByIdAggregateAsync(id);
                if (employee is null) return new(null, 404, "Funcionário não encontrado");
                return new(employee, 200, "Funcionário encontrado");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> CreateAsync(CreateProfileEmployeeRequest request)
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
                    AccessProfile = "clinic-employee",
                    ValidatedAccess = true,
                    Admin = false,
                    Master = false,
                    Photo = request.Photo ?? "",
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                ResponseApi<User?> userResp = await userRepository.CreateAsync(user);
                if (userResp.Data is null) return new(null, 400, "Falha ao criar funcionário");

                ProfileEmployee profile = new()
                {
                    UserId = user.Id,
                    Role = request.Role,
                    Cpf = request.Cpf,
                    RegistrationNumber = request.RegistrationNumber,
                    HireDate = request.HireDate,
                    Address = request.Address ?? new(),
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await repository.CreateAsync(profile);

                dynamic? created = await repository.GetByIdAggregateAsync(user.Id);
                return new(created, 201, "Funcionário cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public async Task<ResponseApi<dynamic?>> UpdateAsync(UpdateProfileEmployeeRequest request)
        {
            try
            {
                ResponseApi<User?> userResp = await userRepository.GetByIdAsync(request.Id);
                User? user = userResp.Data;

                if (user is null)
                {
                    ProfileEmployee? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Funcionário não encontrado");

                if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Email)) user.Email = request.Email;
                if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
                if (!string.IsNullOrEmpty(request.Photo)) user.Photo = request.Photo;
                if (!string.IsNullOrEmpty(request.Password)) user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                if (request.Blocked.HasValue) user.Blocked = request.Blocked.Value;

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = request.UpdatedBy;
                await userRepository.UpdateAsync(user);

                ProfileEmployee? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is null)
                {
                    profile = new()
                    {
                        UserId = user.Id,
                        Role = request.Role ?? "",
                        Cpf = request.Cpf ?? "",
                        RegistrationNumber = request.RegistrationNumber ?? "",
                        HireDate = request.HireDate,
                        Address = request.Address ?? new(),
                        CreatedBy = request.UpdatedBy,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await repository.CreateAsync(profile);
                }
                else
                {
                    if (request.Role is not null) profile.Role = request.Role;
                    if (request.Cpf is not null) profile.Cpf = request.Cpf;
                    if (request.RegistrationNumber is not null) profile.RegistrationNumber = request.RegistrationNumber;
                    if (request.HireDate.HasValue) profile.HireDate = request.HireDate;
                    if (request.Address is not null) profile.Address = request.Address;

                    profile.UpdatedAt = DateTime.UtcNow;
                    profile.UpdatedBy = request.UpdatedBy;
                    await repository.UpdateAsync(profile);
                }

                dynamic? updated = await repository.GetByIdAggregateAsync(user.Id);
                return new(updated, 200, "Funcionário atualizado com sucesso");
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
                    ProfileEmployee? profileCheck = await repository.GetByIdAsync(request.Id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Funcionário não encontrado");

                user.Deleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedBy = request.DeletedBy;
                await userRepository.UpdateAsync(user);

                ProfileEmployee? profile = await repository.GetByUserIdAsync(user.Id);
                if (profile is not null)
                {
                    profile.Deleted = true;
                    profile.DeletedAt = DateTime.UtcNow;
                    profile.DeletedBy = request.DeletedBy;
                    await repository.DeleteAsync(profile);
                }

                return new(new { id = user.Id }, 200, "Funcionário removido com sucesso");
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
                    ProfileEmployee? profileCheck = await repository.GetByIdAsync(id);
                    if (profileCheck is not null)
                    {
                        var userByProfile = await userRepository.GetByIdAsync(profileCheck.UserId);
                        user = userByProfile.Data;
                    }
                }

                if (user is null) return new(null, 404, "Funcionário não encontrado");

                user.Blocked = !user.Blocked;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedBy;
                await userRepository.UpdateAsync(user);

                return new(new { id = user.Id, blocked = user.Blocked }, 200, user.Blocked ? "Funcionário bloqueado com sucesso" : "Funcionário desbloqueado com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }
    }
}
