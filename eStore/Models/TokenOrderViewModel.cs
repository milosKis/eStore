using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eStore.Models
{
    public class TokenOrderViewModel
    {
        [AllZeros]
        [LessThanZeroSilver]
        public int SilverCount { get; set; }

        [AllZeros]
        [LessThanZeroGold]
        public int GoldCount { get; set; }

        [AllZeros]
        [LessThanZeroPlatinum]
        public int PlatinumCount { get; set; }
    }


    public class AllZeros : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (TokenOrderViewModel)validationContext.ObjectInstance;

            if (model.SilverCount == 0 && model.GoldCount == 0 && model.PlatinumCount == 0)
                return new ValidationResult("At least one field value has to be greater than 0");

            return ValidationResult.Success;
        }
    }

    public class LessThanZeroSilver : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (TokenOrderViewModel)validationContext.ObjectInstance;

            if (model.SilverCount < 0)
                return new ValidationResult("Value of this field should be 0 or greater");

            return ValidationResult.Success;
        }
    }

    public class LessThanZeroGold : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (TokenOrderViewModel)validationContext.ObjectInstance;

            if (model.GoldCount < 0)
                return new ValidationResult("Value of this field should be 0 or greater");

            return ValidationResult.Success;
        }
    }

    public class LessThanZeroPlatinum : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (TokenOrderViewModel)validationContext.ObjectInstance;

            if (model.PlatinumCount < 0)
                return new ValidationResult("Value of this field should be 0 or greater");

            return ValidationResult.Success;
        }
    }
}