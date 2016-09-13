using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.CQS
{

    /// <summary>
    /// Used for an output property of an ICommand. This validates that the
    /// property has not been assigned to prior to execution.
    /// </summary>
    public class OutputValueAttribute : ValidationAttribute
    {
        public OutputValueAttribute() :
            base("Output value must not be assigned.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var t = value.GetType();
            var newVal = Convert.ChangeType(value, t);
            var val = Activator.CreateInstance(t);
            if (t.IsValueType && value.Equals(val)) return ValidationResult.Success;

            return new ValidationResult(base.ErrorMessage);
        }
    }
}
