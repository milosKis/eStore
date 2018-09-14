using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public class NumberBiggerThanZero : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var auctionViewModel = (CreateAuctionViewModel) validationContext.ObjectInstance;

            if (auctionViewModel.StartingPrice <= 0 || auctionViewModel.Duration <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            return ValidationResult.Success;
        }
    }
}