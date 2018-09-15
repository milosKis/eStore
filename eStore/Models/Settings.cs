using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eStore.fonts.Models
{
    public class NumberBiggerThanZeroDuration : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.Duration <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            return ValidationResult.Success;
        }
    }

    public class NumberBiggerThanZeroTokenValue : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.TokenValue <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            return ValidationResult.Success;
        }
    }

    public class NumberBiggerThanZeroPageNumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.NumOfItemsPerPage <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            return ValidationResult.Success;
        }
    }

    public class SilverRestrictions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.Silver <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            if ((model.Silver > model.Gold) || (model.Silver > model.Platinum))
                return new ValidationResult("Number of tokens must be less than in Gold or Platinum package");

            return ValidationResult.Success;
        }
    }

    public class GoldRestrictions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.Gold <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            if ((model.Gold < model.Silver) || (model.Gold > model.Platinum))
                return new ValidationResult("Number of tokens must be greater than in Silver and less than in Platinumpackage");

            return ValidationResult.Success;
        }
    }

    public class PlatinumRestrictions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.Platinum <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            if ((model.Platinum < model.Gold) || (model.Platinum < model.Silver))
                return new ValidationResult("Number of tokens must be greater than in Silver or Gold package");

            return ValidationResult.Success;
        }
    }
}