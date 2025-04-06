using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;
using RentACar.Repositories.Helpers;

namespace RentACar.Repositories.Interfaces
{
    public interface IBookingPeriodRepository
    {
        //az
        Task<List<BookingPeriod>> GetConflictingBookingsAsync(int autoId, DateTime pickUpDate, DateTime returnDate);
        Task<bool> IsAutoAvailableAsync(int autoId, DateTime startDate, DateTime endDate);
        Task<BookingResult> BookAutoAsync(int autoId, DateTime startDate, DateTime endDate);
        Task<bool> SaveAsync();
    }
}
