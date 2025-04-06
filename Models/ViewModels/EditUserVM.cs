using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;

namespace RentACar.Data.ViewModels
{
        public class EditUserVM
        {
            public string Id { get; set; }

            [Required(ErrorMessage = "First name is required")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "Surname is required")]
            public string Surname { get; set; }

            [Phone(ErrorMessage = "Invalid phone number")]
            public string PhoneNumber { get; set; }

            [EmailAddress(ErrorMessage = "Invalid email")]
            public string Email { get; set; }

            public string Role { get; set; }
    }
}
