using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Data.Models
{
    public class Auto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int PassengerSeats { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public int PricePerDay { get; set; }

        public virtual List<Request> Requests { get; set; } = new List<Request>();
        public virtual List<BookingPeriod> Bookings { get; set; } = new List<BookingPeriod>();

    }
}
