using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Data.Models;

public class User : IdentityUser
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public int NIN { get; set; }
    [Required]
    public int Number { get; set; }
    [Required]
    public string Email { get; set; }

    public virtual List<Request> Requests { get; set; } = new List<Request>();


}
