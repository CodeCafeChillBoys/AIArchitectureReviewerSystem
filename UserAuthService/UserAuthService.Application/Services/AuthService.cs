using Microsoft.AspNetCore.Identity;
using UserAuthService.Application.Constants;
using UserAuthService.Application.DTOs.Request;
using UserAuthService.Application.DTOs.Response;
using UserAuthService.Application.Interfaces;
using UserAuthService.Domain.Entities;
using UserAuthService.Domain.Interfaces;

namespace UserAuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return ApiResponse<LoginResponse>.FailureResponse(AuthMessages.InvalidCredentials);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return ApiResponse<LoginResponse>.FailureResponse(AuthMessages.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Save refresh token to DB
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken>();
        await refreshTokenRepo.AddAsync(refreshTokenEntity);
        await _unitOfWork.CompleteAsync();

        var loginData = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email ?? string.Empty,
            ExpiresIn = 3600 // 60 mins in seconds
        };

        return ApiResponse<LoginResponse>.SuccessResponse(loginData, AuthMessages.LoginSuccess);
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return ApiResponse<RegisterResponse>.FailureResponse(AuthMessages.UserAlreadyExists);
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RegisterResponse>.FailureResponse(AuthMessages.RegisterFailed, errors);
        }

        // Ensure role exists
        var roleName = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role;
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        await _userManager.AddToRoleAsync(user, roleName);

        var registerData = new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Role = roleName,
            CreatedAt = user.CreatedAt
        };

        return ApiResponse<RegisterResponse>.SuccessResponse(registerData, AuthMessages.RegisterSuccess);
    }
}