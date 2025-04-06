using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentACar.Data.Models;

namespace RentACar.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
    {
    }
    public DbSet<Auto> Autos { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<BookingPeriod> BookingPeriods { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Request>()
            .HasOne(r => r.Auto)
            .WithMany(a => a.Requests)
            .HasForeignKey(r => r.AutoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Request>()
            .HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
           .HasIndex(u => u.NIN)
        .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.Email)
        .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        // Constants for seeded data
        const string adminUserId = "a820ccf9-54ac-4047-b4b5-48dab0dc962b";
        const string adminRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d29";
        const string userRoleId = "a23a7ee8-beb5-4238-ad8a-88d54b3c3d28";
        const string adminConcurrencyStamp = "12345678-1234-1234-1234-123456789012";
        const string userConcurrencyStamp = "12345678-1234-1234-1234-123456789013";
        const string adminPasswordHash = "AQAAAAIAAYagAAAAEJmsXmTvQxGXj8Yzq1uXW5JZ6+7V9kKj1pZ2h3Y4vR4X5nB6r7s8W3Y2w1oA1xg=="; // Hash for "Abc123!"

        // Seed Admin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",
            Id = adminRoleId,
            ConcurrencyStamp = adminConcurrencyStamp
        });

        // Seed User role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "BasicUser",
            NormalizedName = "BASICUSER",
            Id = userRoleId,
            ConcurrencyStamp = userConcurrencyStamp
        });

        builder.Entity<User>().HasData(new User  // Changed from IdentityUser to your User class
        {
            Id = adminUserId,
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            PasswordHash = adminPasswordHash,
            SecurityStamp = string.Empty,
            ConcurrencyStamp = adminConcurrencyStamp,
            PhoneNumber = "+1234567890",  // Now required
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnd = null,
            LockoutEnabled = true,
            AccessFailedCount = 0,

            // ADD YOUR CUSTOM FIELDS HERE
            Firstname = "Admin",
            Surname = "User",
            NIN = "ADMIN123456" // Or any valid format
        });

        // Set user role to admin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUserId
        });
    }
}