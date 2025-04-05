using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;

namespace RentACar.Repositories.Interfaces
{
    public interface IAutoRepository
    {
        //az
        Task<IEnumerable<Auto>> GetAllAutosAsync();
        Task<IEnumerable<Auto>> GetAllAutosFreeAsync(DateTime pickUpDate, DateTime returnDate);
        Task<Auto> GetAutoByIdAsync(int autoId);
        Task<bool> AddAsync(Auto auto);
        Task<bool> UpdateAsync(Auto auto);
        Task<bool> DeleteAsync(Auto auto);
        Task<bool> SaveAsync();
        // search po brand/passenger seats?
    }
}
