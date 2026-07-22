using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserAuthService.Application.DTOs;

namespace UserAuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive);
        Task<bool> UpdateUserRoleAsync(Guid userId, int role);
    }
}
