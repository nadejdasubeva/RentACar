using System.Runtime.Intrinsics.Arm;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Data.Models;
using RentACar.Repositories.Helpers;
using RentACar.Repositories.Interfaces;

namespace RentACar.Repositories
{
    public class BookingPeriodRepository : IBookingPeriodRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingPeriodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResult> BookAutoAsync(int autoId, DateTime startDate, DateTime endDate)
        {
            bool isAvailable = await IsAutoAvailableAsync(autoId, startDate, endDate);

            if (!isAvailable)
            {
                return new BookingResult
                {
                    Success = false,
                    Message = "Auto is not available for the selected dates"
                };
            }

            BookingPeriod booking = new BookingPeriod
            {
                AutoId = autoId,
                StartDate = startDate,
                EndDate = endDate
            };

            _context.BookingPeriods.Add(booking);
            await SaveAsync();

            return new BookingResult { Success = true, Booking = booking };
        }

        public async Task<bool> IsAutoAvailableAsync(int autoId, DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                throw new ArgumentException("End date must be after start date");
            }
            else
            {
                    return !await _context.BookingPeriods
                    .AnyAsync(b => b.AutoId == autoId &&
                    b.StartDate < endDate &&
                    b.EndDate > startDate);
            }    
        }
        public async Task<List<BookingPeriod>> GetConflictingBookingsAsync(int autoId, DateTime pickUpDate, DateTime returnDate)
        {
            return await _context.BookingPeriods
                .Where(bp => bp.AutoId == autoId &&
                            pickUpDate <= bp.EndDate &&
                            returnDate >= bp.StartDate)
                .ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false; ;
        }
    }
}
