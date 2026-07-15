using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserAuthService.Domain.Entities;

public partial class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    public string Email { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Fullname { get; set; }

    public string? Avatarurl { get; set; }

    public string? Phone { get; set; }

    public int Role { get; set; }

    public bool? Isactive { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Refreshtoken> Refreshtokens { get; set; } = new List<Refreshtoken>();
}
