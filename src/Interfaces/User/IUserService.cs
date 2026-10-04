

using api_clinic.src.Models;
using api_clinic.src.Models.Base;
using api_clinic.src.Requests;
using api_clinic.src.Shared.DTOs;

namespace api_clinic.src.Interfaces
{
    public interface IUserService
    {
        Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request);
        Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id);
        Task<ResponseApi<List<User>>> GetNotByIdAsync(string id);
        Task<ResponseApi<User?>> CreateAsync(CreateUserDTO user);
        Task<ResponseApi<User?>> UpdateAsync(UpdateUserDTO user);
        Task<ResponseApi<User?>> UpdateConfirmAccountAsync(UpdateConfirmAccountDTO request);
        Task<ResponseApi<dynamic?>> UpdateFCMAsync(UpdateFCMUserDTO request);
        Task<ResponseApi<string>> ProfilePhotoAsync(ProfilePhotoDTO request);
        Task<ResponseApi<string>> RemoveProfilePhotoAsync(ProfilePhotoDTO request);
        Task<ResponseApi<User>> DeleteAsync(DeleteDTO request);
        Task<ResponseApi<User?>> ToggleBlockAsync(string id);
    }
}