using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Domain.Entities;

namespace UserAuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/auth/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                role = roles.FirstOrDefault() ?? "User",
                createdAt = user.CreatedAt
            });
        }

        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound(new { message = "User not found" });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            role = roles.FirstOrDefault() ?? "User",
            createdAt = user.CreatedAt
        });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new { message = "Role is required" });
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound(new { message = "User not found" });

        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = request.Role });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, request.Role);

        return Ok(new { message = "Role updated successfully", role = request.Role });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound(new { message = "User not found" });

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to delete user" });
        }

        return Ok(new { message = "User deleted successfully" });
    }
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
