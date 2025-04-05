using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Repositories.Helpers;

namespace RentACar.Repositories.Interfaces
{
    public interface IBookingPeriodRepository
    {
        Task<bool> IsAutoAvailableAsync(int autoId, DateTime startDate, DateTime endDate);
        Task<BookingResult> BookAutoAsync(int autoId, DateTime startDate, DateTime endDate);
    }
}
