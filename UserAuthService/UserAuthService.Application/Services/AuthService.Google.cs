using FirebaseAdmin.Auth;
using UserAuthService.Application.Constants;
using UserAuthService.Application.DTOs.Request;
using UserAuthService.Application.DTOs.Response;
using UserAuthService.Application.Validation;
using UserAuthService.Domain.Entities;

namespace UserAuthService.Application.Services;

public partial class AuthService
{
    public async Task<ApiResponse<LoginResponse>> GoogleLoginAsync(GoogleLoginRequest request)
    {
        // 1. Verify IdToken nhận được từ Frontend bằng Firebase SDK
        FirebaseToken decodedToken;
        try
        {
            decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
        }
        catch (Exception ex)
        {
            return ApiResponse<LoginResponse>.FailureResponse(AuthMessages.InvalidGoogleToken, new List<string> { ex.Message });
        }

        // 2. Trích xuất Email từ Token
        var email = decodedToken.Claims.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
        if (string.IsNullOrEmpty(email))
        {
            return ApiResponse<LoginResponse>.FailureResponse(AuthMessages.GoogleEmailNotFound);
        }

        // 3. Tìm User trong DB. Nếu chưa có -> Tạo mới và phân Role tự động
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse<LoginResponse>.FailureResponse(AuthMessages.RegisterFailed, errors);
            }

            // Phân biệt "Student" vs "Lecturer" qua EmailRoleValidator
            string roleName = EmailRoleValidator.IsStudentEmail(email) ? "User" : "Admin";

            // Đảm bảo Role đã có trong AspNetRoles
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }

            // Gán Role cho User
            await _userManager.AddToRoleAsync(user, roleName);
        }

        // 4. Phát hành JWT AccessToken & RefreshToken của hệ thống
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // Lưu RefreshToken vào DB
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

        // Trả về kết quả LoginResponse
        var loginData = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email ?? string.Empty,
            ExpiresIn = 3600
        };

        return ApiResponse<LoginResponse>.SuccessResponse(loginData, AuthMessages.LoginSuccess);
    }
}