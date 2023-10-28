using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Responses;

namespace Calendar.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest);

        Task<ServiceResult> Logout();

        Task<ServiceResult<LoginResponse>> Register(RegisterUserRequest registerRequest);
    }
}