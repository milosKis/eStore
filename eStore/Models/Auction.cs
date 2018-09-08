using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public class Auction
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public byte[] Image { get; set; }

        [Required]
        public long Duration { get; set; }

        [Required]
        [Display(Name = "Starting price")]
        public double StartingPrice { get; set; }

        [Required]
        [Display(Name = "Current price")]
        public double CurrentPrice { get; set; }

        [Required]
        [Display(Name = "Created")]
        public DateTime DateTimeCreated { get; set; }

        [Display(Name = "Opened")]
        public DateTime? DateTimeOpened { get; set; }

        [Display(Name = "Closed")]
        public DateTime? DateTimeClosed { get; set; }

        [Required]
        [StringLength(10)]
        public string State { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}