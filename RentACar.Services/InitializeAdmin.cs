using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RentACar.Data.Models;

namespace RentACar.Services
{
    public static class InitializeAdmin
    {
        public static async Task InitializeAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure admin role exists
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            // Check for existing admin
            var adminEmail = "admin@admin.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin != null)
            {
                // Update existing admin
                existingAdmin.Firstname = "Admin";
                existingAdmin.Surname = "User";
                existingAdmin.NIN = "ADMIN123456";
                existingAdmin.PhoneNumber = "+1234567890";
                existingAdmin.EmailConfirmed = true;
                existingAdmin.PhoneNumberConfirmed = true;

                // Reset password
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
                var passwordResult = await userManager.ResetPasswordAsync(
                    existingAdmin,
                    resetToken,
                    "SecurePassword123!");

                if (!passwordResult.Succeeded)
                {
                    throw new Exception($"Password reset failed: {string.Join(", ", passwordResult.Errors)}");
                }

                // Save user updates
                var updateResult = await userManager.UpdateAsync(existingAdmin);
                if (!updateResult.Succeeded)
                {
                    throw new Exception($"Admin update failed: {string.Join(", ", updateResult.Errors)}");
                }
            }
            else
            {
                // Create new admin user if none exists
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Firstname = "Admin",
                    Surname = "User",
                    NIN = "ADMIN123456",
                    PhoneNumber = "+1234567890",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "SecurePassword123!");
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Admin creation failed: {string.Join(", ", createResult.Errors)}");
                }

                // Assign admin role to new user
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }

            // Ensure admin has the Administrator role (for both new and existing)
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }
    }
}
