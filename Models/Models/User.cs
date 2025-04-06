using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public string Firstname { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string NIN { get; set; }
    [Required]
    public override string UserName { get; set; }
    [Required]
    public override string PhoneNumber { get; set; }
    [Required]
    public override string Email { get; set; }

    public virtual List<Request> Requests { get; set; } = new List<Request>();
}