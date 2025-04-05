using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RentACar.Data.Models;
using RentACar.Services.Interfaces;

namespace RentACar.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ChangeRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            if (await _userManager.IsInRoleAsync(user, "BasicUser"))
            {
                await _userManager.RemoveFromRoleAsync(user, "BasicUser");
                await _userManager.AddToRoleAsync(user, "Administrator");
            }
            else if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Administrator");
                await _userManager.AddToRoleAsync(user, "BasicUser");
            }

            return true;
        }
    }

}
