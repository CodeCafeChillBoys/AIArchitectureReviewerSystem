using UserAuthService.Application.DTOs.Request;
using UserAuthService.Application.DTOs.Response;

namespace UserAuthService.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
}
