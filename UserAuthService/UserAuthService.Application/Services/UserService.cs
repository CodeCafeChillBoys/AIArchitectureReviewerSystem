using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Application.DTOs;
using UserAuthService.Application.Interfaces;
using UserAuthService.Infrastructure.Persitence.Data;

namespace UserAuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserAuthServiceDbContext _context;

        public UserService(UserAuthServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Fullname = u.Fullname,
                    Avatarurl = u.Avatarurl,
                    Phone = u.Phone,
                    Role = u.Role,
                    Isactive = u.Isactive,
                    Createdat = u.Createdat,
                    Updatedat = u.Updatedat
                })
                .ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var u = await _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
            if (u == null) return null!;

            return new UserDto
            {
                UserId = u.UserId,
                Email = u.Email,
                Fullname = u.Fullname,
                Avatarurl = u.Avatarurl,
                Phone = u.Phone,
                Role = u.Role,
                Isactive = u.Isactive,
                Createdat = u.Createdat,
                Updatedat = u.Updatedat
            };
        }

        public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            user.Isactive = isActive;
            user.Updatedat = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(Guid userId, int role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            user.Role = role;
            user.Updatedat = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
