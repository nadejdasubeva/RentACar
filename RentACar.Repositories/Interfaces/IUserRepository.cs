using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentACar.Data.Models;

namespace RentACar.Repositories.Interfaces
{
    public interface IUserRepository
    {
        //az
        Task<bool> AnyAsync(Expression<Func<User, bool>> predicate);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(string id);
        Task<bool> AddAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<bool> SaveAsync();
    }
}
