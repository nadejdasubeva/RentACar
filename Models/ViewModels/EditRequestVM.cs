using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar.Data.Helpers;
using RentACar.Data.Models;
using static RentACar.Data.Helpers.FutureDateAttribute;

namespace RentACar.Data.ViewModels
{
    public class EditRequestVM
    {
        public int Id { get; set; }  // Required for edit operations

        [Required(ErrorMessage = "Vehicle selection is required")]
        [Display(Name = "Vehicle")]
        public int AutoId { get; set; }

        // Display only - not editable
        [Display(Name = "Current Vehicle")]
        public string CurrentAutoDisplay { get; set; }

        [Required(ErrorMessage = "Pick-up date is required")]
        [Display(Name = "Pick-Up Date")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Pick-up date must be in the future")]
        public DateTime PickUpDate { get; set; }

        [Required(ErrorMessage = "Return date is required")]
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DateAfter(nameof(PickUpDate), ErrorMessage = "Return date must be after pick-up date")]
        public DateTime ReturnDate { get; set; }

        // System fields (display only)
        [Display(Name = "Request Date")]
        public DateTime DateOfRequest { get; set; }

        [Display(Name = "Requested By")]
        public string UserName { get; set; }

        // Approval status (for admin only)
        [Display(Name = "Approval Status")]
        public bool IsApproved { get; set; }

        [Display(Name = "Rejection Status")]
        public bool IsDeclined { get; set; }

        // Calculated properties (readonly)
        [Display(Name = "Total Rental Days")]
        public int RentalDays => (ReturnDate - PickUpDate).Days;

        [Display(Name = "Original Request Date")]
        public DateTime OriginalRequestDate { get; set; }
    }
}