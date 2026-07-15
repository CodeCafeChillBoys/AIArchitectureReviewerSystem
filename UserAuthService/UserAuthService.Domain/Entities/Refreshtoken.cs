using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserAuthService.Domain.Entities;

public partial class Refreshtoken
{
    [Key]
    public Guid TokenId { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expiresat { get; set; }

    public DateTime? Createdat { get; set; }

    public bool? Revoked { get; set; }

    public virtual User User { get; set; } = null!;
}
