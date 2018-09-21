using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public class CreateAuctionViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Upload Photo")]
        public HttpPostedFileBase ImageFile { get; set; }

        [Required]
        [NumberBiggerThanZeroDuration]
        public long Duration { get; set; }

        [Required]
        [NumberBiggerThanZeroStartingPrice]
        [Display(Name = "Starting price")]
        public double StartingPrice { get; set; }
    }
}