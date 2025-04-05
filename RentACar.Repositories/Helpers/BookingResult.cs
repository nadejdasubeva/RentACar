using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;

namespace RentACar.Repositories.Helpers
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public BookingPeriod Booking { get; set; }
    }
}
