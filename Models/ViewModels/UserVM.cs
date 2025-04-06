using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;

namespace RentACar.Data.ViewModels
{
    public class UserVM
    {
            public string Id { get; set; }
            [Required(ErrorMessage = "First name is required.")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Surname is required.")]
            public string Surname { get; set; }

            [Required(ErrorMessage = "National Identification Number is required.")]
            [StringLength(20, MinimumLength = 5, ErrorMessage = "NIN must be between 5 and 20 characters.")]
            public string NIN { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [Phone(ErrorMessage = "Invalid phone number format.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address format.")]
            public string Email { get; set; }
            public string Role { get; set; }

            public virtual List<Request>? Requests { get; set; }

    }
}
