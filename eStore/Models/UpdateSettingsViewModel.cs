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
        [NumberBiggerThanZeroSettings]
        [Display(Name = "Default auction's duration")]
        public int Duration { get; set; }

        [Required]
        [NumberBiggerThanZeroSettings]
        [Display(Name = "Token value")]
        public double TokenValue { get; set; }

        [Required]
        [NumberBiggerThanZeroSettings]
        [Display(Name = "Number of items per page")]
        public double NumOfItemsPerPage { get; set; }

        public string Message { get; set; }


    }
}