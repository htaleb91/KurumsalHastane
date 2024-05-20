using System.ComponentModel.DataAnnotations;

namespace KurumsalHastane.Infrastructure.Validation
{
    public class NullableIfIdEqualZero : ValidationAttribute
    {
        private readonly string _conditionPropertyName;
        private readonly object _conditionValue;

        public NullableIfIdEqualZero(string conditionPropertyName, object conditionValue)
        {
            _conditionPropertyName = conditionPropertyName;
            _conditionValue = conditionValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var conditionProperty = validationContext.ObjectType.GetProperty(_conditionPropertyName);
            if (conditionProperty == null)
            {
                return new ValidationResult($"Unknown property: {_conditionPropertyName}");
            }

            var conditionPropertyValue = conditionProperty.GetValue(validationContext.ObjectInstance);

            if (conditionPropertyValue != null && conditionPropertyValue.Equals(_conditionValue))
            {
                if (value is string stringValue && stringValue == null)
                {
                    return new ValidationResult($"The field {validationContext.DisplayName} cannot be null when {_conditionPropertyName} is {_conditionValue}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}