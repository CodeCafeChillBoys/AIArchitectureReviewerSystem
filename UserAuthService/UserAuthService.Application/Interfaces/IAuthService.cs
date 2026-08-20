using Microsoft.AspNetCore.Identity.Data;
using UserAuthService.Application.DTOs.Request;
using UserAuthService.Application.DTOs.Response;
using LoginRequest = UserAuthService.Application.DTOs.Request.LoginRequest;
using RegisterRequest = UserAuthService.Application.DTOs.Request.RegisterRequest;

namespace UserAuthService.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<LoginResponse>> GoogleLoginAsync(GoogleLoginRequest request);

}

