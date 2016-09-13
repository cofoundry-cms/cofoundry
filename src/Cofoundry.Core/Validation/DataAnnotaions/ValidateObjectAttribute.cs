using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Validates a child object property
    /// </summary>
    /// <remarks>
    /// Taken from http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.html
    /// </remarks>
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // if this a collection, validate each item.
            if (value is IEnumerable)
            {
                foreach (object collectionValue in (IEnumerable)value)
                {
                    ValidateValue(collectionValue, results);
                }
            }
            else
            {
                ValidateValue(value, results);
            }
            
            if (results.Count != 0)
            {
                var msg = String.Format("{0} validation failed", validationContext.DisplayName);
                var compositeResults = new CompositeValidationResult(msg, new string[] { validationContext.MemberName });
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }

        private void ValidateValue(object value, List<ValidationResult> results)
        {
            // ValidationContext constructor requires value to not be null.
            if (value == null) return;

            var context = new ValidationContext(value, null, null);

            Validator.TryValidateObject(value, context, results, true);
        }
    }
}
