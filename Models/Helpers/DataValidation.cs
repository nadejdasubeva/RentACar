using System;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Data.Helpers
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                // Compare dates (ignoring time)
                if (date.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be in the future");
                }
            }
            return ValidationResult.Success;
        }
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class DateAfterAttribute : ValidationAttribute
        {
            private readonly string _comparisonProperty;

            public DateAfterAttribute(string comparisonProperty)
            {
                _comparisonProperty = comparisonProperty;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                // 1. Get the comparison property
                var propertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
                if (propertyInfo == null)
                {
                    return new ValidationResult($"Unknown property: {_comparisonProperty}");
                }

                // 2. Get the comparison value
                var comparisonValue = (DateTime?)propertyInfo.GetValue(validationContext.ObjectInstance);

                // 3. Validate
                if (value is DateTime dateValue && comparisonValue.HasValue)
                {
                    if (dateValue.Date <= comparisonValue.Value.Date)
                    {
                        return new ValidationResult(
                            ErrorMessage ?? $"{validationContext.DisplayName} must be after {_comparisonProperty}");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }
}