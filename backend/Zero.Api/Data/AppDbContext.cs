using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zero.Api.Models.Auth;

namespace Zero.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Esquema auth
        var schema = "Auth";

        builder.Entity<AppUser>().ToTable("Users", schema);
        builder.Entity<IdentityRole>().ToTable("Roles", schema);
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema);
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema);
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema);
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema);
    }
}