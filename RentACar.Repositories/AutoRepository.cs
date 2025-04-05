using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Data.Models;
using RentACar.Repositories.Interfaces;

namespace RentACar.Repositories
{
    public class AutoRepository : IAutoRepository
    {
        private readonly ApplicationDbContext _context;
        public AutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(Auto auto)
        {
            _context.Add(auto);
            return await SaveAsync();
        }
        public async Task<bool> DeleteAsync(Auto auto)
        {
            _context.Remove(auto);
            return await SaveAsync();
        }
        public async Task<IEnumerable<Auto>> GetAllAutosAsync()
        {
            return await _context.Autos.ToListAsync();
        }
        public async Task<IEnumerable<Auto>> GetAllAutosFreeAsync(DateTime startDate, DateTime endDate)
        {
            // Validate date range
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");

            // Get all autos that don't have conflicting bookings
            return await _context.Autos
                .Where(a => !a.Bookings.Any(b =>
                    b.StartDate < endDate &&
                    b.EndDate > startDate))
                .ToListAsync();
        }

        public async Task<Auto> GetAutoByIdAsync(int autoId)
        {
            return await _context.Autos
                .FirstOrDefaultAsync(a => a.Id == autoId);

        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
        public async Task<bool> UpdateAsync(Auto auto)
        {
            _context.Update(auto);
            return await SaveAsync();
        }
    }


}
