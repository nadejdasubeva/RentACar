using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Data.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AutoId")]
        public int AutoId { get; set; }
        public Auto Auto { get; set; }

        [Required]
        public DateTime PickUpDate { get; set; }

        [Required]
        public DateTime ReturnDate { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsDeclined { get; set; } = false;
        [Required]
        public DateTime DateOfRequest { get; set; }
        public User User { get; set; }

    }
}