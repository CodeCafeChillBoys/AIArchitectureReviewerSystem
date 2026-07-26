using System;

namespace UserAuthService.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Fullname { get; set; }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public int Role { get; set; }
    }

    public class ProfileResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? Fullname { get; set; }
        public string? Avatarurl { get; set; }
        public string? Phone { get; set; }
        public int Role { get; set; }
    }
}
