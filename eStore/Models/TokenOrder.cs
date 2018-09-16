using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace eStore.Models
{
    public class TokenOrder
    {
        public int Id { get; set; }

        [Required]
        public int NumOfTokens { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}