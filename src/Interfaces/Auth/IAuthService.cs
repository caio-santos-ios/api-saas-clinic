using api_clinic.src.Models.Base;
using api_clinic.src.Requests;
using api_clinic.src.Requests.User;

namespace api_clinic.src.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseApi<dynamic?>> CreateUserAdminAsync(CreateUserAdminRequest request);
        Task<ResponseApi<dynamic?>> CreateAsync(CreateUserDTO request);
        Task<ResponseApi<dynamic?>> LoginAsync(LoginRequest request);
        Task<ResponseApi<dynamic?>> NewCodeAsync(NewCodeRequest request);
        Task<ResponseApi<dynamic?>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ResponseApi<dynamic?>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ResponseApi<dynamic?>> CleanIncorrectPasswordAsync(CleanIncorrectPasswordRequest request);
        Task<ResponseApi<dynamic?>> GetThemeByCodeAsync(string code);
        Task<ResponseApi<dynamic?>> GetThemeByClinicIdAsync(string clinicId);
    }
}