using System;

namespace UserAuthService.Application.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string? Fullname { get; set; }
        public string? Avatarurl { get; set; }
        public string? Phone { get; set; }
        public int Role { get; set; }
        public bool? Isactive { get; set; }
        public DateTime? Createdat { get; set; }
        public DateTime? Updatedat { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public bool Isactive { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public int Role { get; set; }
    }
}
