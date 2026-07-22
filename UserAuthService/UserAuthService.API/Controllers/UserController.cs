using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAuthService.Application.Interfaces;
using UserAuthService.Application.DTOs;

namespace UserAuthService.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UserController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized("Invalid token claims.");
                }

                var profile = await _authService.GetProfileAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
        {
            var result = await _userService.UpdateUserStatusAsync(id, dto.Isactive);
            if (!result) return NotFound("User not found.");
            return Ok(new { Message = "Cập nhật trạng thái thành công." });
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto dto)
        {
            var result = await _userService.UpdateUserRoleAsync(id, dto.Role);
            if (!result) return NotFound("User not found.");
            return Ok(new { Message = "Cập nhật vai trò thành công." });
        }
    }
}
