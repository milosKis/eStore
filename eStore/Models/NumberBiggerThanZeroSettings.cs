using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eStore.fonts.Models
{
    public class NumberBiggerThanZeroSettings : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (UpdateSettingsViewModel)validationContext.ObjectInstance;

            if (model.Duration <= 0 || model.NumOfItemsPerPage <= 0 || model.TokenValue <= 0)
                return new ValidationResult("Value of this field has to be greater than 0");

            return ValidationResult.Success;
        }
    }
}