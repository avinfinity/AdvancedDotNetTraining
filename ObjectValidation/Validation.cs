using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectValidation
{

    public class PropertyValidationResultAggregator
    {
        private readonly IEnumerable<PropertyValidationResult> _ValidationResult;
        public PropertyValidationResultAggregator(IEnumerable<PropertyValidationResult> validationResult)
        {
            _ValidationResult = validationResult;
        }

        public bool IsValid
        {
            get
            {
                return _ValidationResult.Count() > 0;
            }
        }

        public IEnumerable<PropertyValidationResult> Results
        {
            get
            {
                return _ValidationResult;
            }
        }
    }

    public class PropertyValidationResult
    {
        public PropertyValidationResult(string propName, string messages)
        {
            PropertyName = propName;
            ErrorMessage = messages;
        }

        public string PropertyName { get; private set; }

        public string ErrorMessage { get; private set; }
    }

    public static class Validator
    {
      public static PropertyValidationResultAggregator Validate(object tobeValidated)
        {
            IList<PropertyValidationResult> propertyValidationResults = new List<PropertyValidationResult>();

            var properties = tobeValidated.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                var validationAttributes = properties[i].GetCustomAttributes(typeof(ValidationAttribute)).OfType<ValidationAttribute>();
                foreach (var attribute in validationAttributes)
                {
                    if (!attribute.Validate(tobeValidated))
                        propertyValidationResults.Add(new PropertyValidationResult(properties[i].Name, attribute.ErrorMessage));
                }
            }
            return new PropertyValidationResultAggregator(propertyValidationResults);
        }
    }

    public static class ValidatorExtensions
    {
        public static PropertyValidationResultAggregator Validate(this object tobeValidated)
        {
            var propertiesToBeValidated = tobeValidated
                                            .GetType()
                                            .GetProperties()
                                            .Select(prop => prop.GetCustomAttributes(typeof(ValidationAttribute)).FirstOrDefault())
                                            .OfType<ValidationAttribute>();

            var propertiesWhichFailed = propertiesToBeValidated
                .Where(pro => !pro.Validate(tobeValidated))
                .Select(obj => new PropertyValidationResult(obj.TypeId.ToString(), obj.ErrorMessage));

            return new PropertyValidationResultAggregator(propertiesWhichFailed);
        }
    }
}