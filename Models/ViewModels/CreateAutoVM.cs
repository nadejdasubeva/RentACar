using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace RentACar.Data.ViewModels
{

    public class CreateAutoVM
    {
        [Required(ErrorMessage = "Brand is required")]
        [StringLength(50, ErrorMessage = "Brand cannot be longer than 50 characters")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot be longer than 50 characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Number of passenger seats is required")]
        [Range(1, 12, ErrorMessage = "Seats must be between 1 and 12")]
        public int PassengerSeats { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price per day is required")]
        [Range(1, 10000, ErrorMessage = "Price must be between 1 and 10,000")]
        public int PricePerDay { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }


}
