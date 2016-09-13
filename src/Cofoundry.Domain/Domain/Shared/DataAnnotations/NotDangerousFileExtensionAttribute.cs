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
    /// Validates that a path or filename property doesn't contain an extension on a blacklist of extensions
    /// for dangerous file types. 
    /// </summary>
    public class NotDangerousFileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is string))
            {
                throw new ArgumentException("NotDangerousFileTypeValidationAttribute can only be applied to a string property", "value");
            }

            var fileName = (string)value;

            if (!string.IsNullOrEmpty(fileName) && Cofoundry.Core.DangerousFileConstants.DangerousFileExtensions.Contains(Path.GetExtension(fileName)))
            {
                return new ValidationResult("The type of file you're trying to upload isn't allowed.", new string[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
