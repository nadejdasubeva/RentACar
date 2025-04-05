using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Data.Models
{
        public class BookingPeriod
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public DateTime StartDate { get; set; }
            [Required]
            public DateTime EndDate { get; set; }
            [ForeignKey("AutoId")]
            public int AutoId { get; set; }
            public Auto Auto { get; set; }
        }
    
}

