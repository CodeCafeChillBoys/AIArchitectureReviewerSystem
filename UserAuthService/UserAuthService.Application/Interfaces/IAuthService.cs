using System;
using System.Threading.Tasks;
using UserAuthService.Application.DTOs;

namespace UserAuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<ProfileResponseDto> GetProfileAsync(Guid userId);
    }
}
