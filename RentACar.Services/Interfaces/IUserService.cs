using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> ChangeRoleAsync(string userId);
        Task<bool> SetRoleAsync(string userId, string roleName);
    }

}
