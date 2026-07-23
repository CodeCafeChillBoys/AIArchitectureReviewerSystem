using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Application.DTOs;
using UserAuthService.Application.Interfaces;
using UserAuthService.Domain.Entities;
using UserAuthService.Domain.INum;
using UserAuthService.Infrastructure.Persitence.Data;
using BCrypt.Net;

namespace UserAuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserAuthServiceDbContext _context;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(UserAuthServiceDbContext context, IJwtProvider jwtProvider)
        {
            _context = context;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                throw new Exception("Email already exists");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.Fullname,
                Role = (int)UserRole.Student,
                Isactive = true,
                Createdat = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtProvider.GenerateToken(user);
            return new AuthResponseDto
            {
                AccessToken = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
                throw new Exception("Invalid email or password");

            if (user.Isactive == false)
                throw new Exception("User account is inactive");

            var token = _jwtProvider.GenerateToken(user);
            return new AuthResponseDto
            {
                AccessToken = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role
            };
        } 

        public async Task<ProfileResponseDto> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                throw new Exception("User not found");

            return new ProfileResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Fullname = user.Fullname,
                Avatarurl = user.Avatarurl,
                Phone = user.Phone,
                Role = user.Role
            };
        }
    }
}
