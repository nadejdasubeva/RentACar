using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RentACar.Data.Models;
using RentACar.Repositories.Interfaces;
using RentACar.Services.Interfaces;

namespace RentACar.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<bool> ChangeRoleAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Toggle between roles
            var newRole = currentRoles.Contains("Administrator") ? "BasicUser" : "Administrator";
            await _userManager.AddToRoleAsync(user, newRole);

            return true;
        }

        public async Task<bool> SetRoleAsync(string userId, string roleName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, roleName);

            return true;
        }
    }

}
