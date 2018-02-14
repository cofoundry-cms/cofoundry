using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Validates that a mime-type property isnt on a blacklist of mime types
    /// for dangerous file types.
    /// </summary>
    public class NotDangerousMimeTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string))
            {
                throw new ArgumentException("NotDangerousMimeTypeValidationAttribute can only be applied to a string property", nameof(value));
            }

            var mimeType = (string)value;

            if (!string.IsNullOrEmpty(mimeType) && Cofoundry.Core.DangerousFileConstants.DangerousMimeTypes.Contains(mimeType))
            {
                return new ValidationResult("The type of file you're trying to upload isn't allowed.", new string[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
