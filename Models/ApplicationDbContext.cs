using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Data.Models;

namespace RentACar.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Auto> Autos { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        string adminUserId = "a820ccf9-54ac-4047-b4b5-48dab0dc962b";
        string adminRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d29";

        string userRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d28";

        // Seed Admin role.
        builder
            .Entity<IdentityRole>()
            .HasData(new IdentityRole
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Id = adminRoleId,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });

        builder
           .Entity<IdentityRole>()
           .HasData(new IdentityRole
           {
               Name = "BasicUser",
               NormalizedName = "BASICUSER",
               Id = userRoleId,
               ConcurrencyStamp = Guid.NewGuid().ToString()
           });

        // Create admin user.
        var appUser = new IdentityUser
        {
            Id = adminUserId,
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM"
        };

        // Set initial admin password.
        var ph = new PasswordHasher<IdentityUser>();

        // Don't forget to change admin password after initial account creation.
        appUser.PasswordHash = ph.HashPassword(appUser, "Abc123!");

        // Seed user.
        builder
            .Entity<IdentityUser>()
            .HasData(appUser);

        // Set user role to admin.
        builder
            .Entity<IdentityUserRole<string>>()
            .HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
    }
}
