using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Domain.Entities;

namespace UserAuthService.Infrastructure.Persitence.Data;

public partial class UserAuthServiceDbContext : DbContext
{
    public UserAuthServiceDbContext()
    {
    }

    public UserAuthServiceDbContext(DbContextOptions<UserAuthServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    


}
