using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using eStore.Models;

namespace eStore.fonts.Models
{
    public class UpdateSettingsViewModel
    {
        [Display(Name = "Currencies")]
        public string Currencies { get; set; }

        [Required]
        [Display(Name = "Current currency")]
        public string CurrentCurrency { get; set; }

        [Required]
        [NumberBiggerThanZeroDuration]
        [Display(Name = "Default auction's duration")]
        public int Duration { get; set; }

        [Required]
        [NumberBiggerThanZeroTokenValue]
        [Display(Name = "Token value")]
        public double TokenValue { get; set; }

        [Required]
        [SilverRestrictions]
        [Display(Name = "Silver package")]
        public int Silver { get; set; }

        [Required]
        [GoldRestrictions]
        [Display(Name = "Gold package")]
        public int Gold { get; set; }

        [Required]
        [PlatinumRestrictions]
        [Display(Name = "Platinum package")]
        public int Platinum { get; set; }

        [Required]
        [NumberBiggerThanZeroPageNumber]
        [Display(Name = "Number of items per page")]
        public double NumOfItemsPerPage { get; set; }

        public string Message { get; set; }


    }
}