using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace eStore.Models
{
    public class Bid
    {
        public int Id { get; set; }

        [Required]
        public DateTime DateTimeCreated { get; set; }

        [Required]
        public double NumOfTokens { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Auction Auction { get; set; }
    }
}