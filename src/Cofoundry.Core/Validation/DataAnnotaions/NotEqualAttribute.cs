using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Ensures a propertys value does not match another properties value.
    /// </summary>
    /// <remarks>
    /// Adapted from http://stackoverflow.com/a/5742494/716689
    /// </remarks>
    public class NotEqualAttribute : ValidationAttribute
    {
        public string OtherProperty { get; private set; }
        public NotEqualAttribute(string otherProperty)
        {
            OtherProperty = otherProperty;
            ErrorMessage = "The {0} field cannot be the same value as the " + otherProperty + " field";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetTypeInfo().GetProperty(OtherProperty);
            if (property == null)
            {
                throw new ArgumentException(OtherProperty + " is not a property of " + validationContext.ObjectType.FullName);
            }

            var otherValue = property.GetValue(validationContext.ObjectInstance, null);
            if (object.Equals(value, otherValue))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }

            return null;
        }
    }
}
