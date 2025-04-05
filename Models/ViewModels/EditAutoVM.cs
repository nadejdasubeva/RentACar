using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RentACar.Data.ViewModels
{
    public class EditAutoVM
    {
        public int Id { get; set; }  // Required for edit operations

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900-2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Passenger seats are required")]
        [Range(1, 12, ErrorMessage = "Seats must be 1-12")]
        public int PassengerSeats { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per day is required")]
        [Range(1, 10000, ErrorMessage = "Price must be $1-$10,000")]
        public int PricePerDay { get; set; }

        public IFormFile Image { get; set; }
        public string? URL { get; set; }

    }
}
