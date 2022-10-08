using System;
using System.ComponentModel.DataAnnotations;

namespace KitchenStock.Shared.CustomValidations
{
    /// <summary>
    /// Custom atttribute to verify that the associated DateTime (if contains a not-null value) has a future date
    /// </summary>
    internal class FutureDateAttribute : ValidationAttribute
    {
        const string CustomErrorMessage = "The inserted date MUST be in the future";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stock = (StockItem)validationContext.ObjectInstance;
            if (!stock.NextRefillDate.HasValue || (stock.NextRefillDate.HasValue && stock.NextRefillDate.Value > DateTime.UtcNow))
                return ValidationResult.Success;

            return new ValidationResult(CustomErrorMessage);
        }
    }
}
