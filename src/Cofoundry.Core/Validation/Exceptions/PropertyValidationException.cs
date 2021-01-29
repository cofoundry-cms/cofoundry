using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// A validation exception that relates to a specific property or number of properties 
    /// on an object
    /// </summary>
    [Obsolete("Use ValidationErrorException or ValidationErrorException.NewWithProperties()")]
    public class PropertyValidationException : ValidationException
    {
        /// <summary>
        /// Creates a new PropertyValidationException relating to multiple properties
        /// </summary>
        /// <param name="message">The message to assign to the exception</param>
        /// <param name="properties">The properties that failed validation.</param>
        /// <param name="value">Optionally value of the object/properties that caused the attribute to trigger validation error.</param>
        public PropertyValidationException(string message, IEnumerable<string> properties, object value = null)
            : base(GetValidationResult(message, properties), null, value)
        {
        }

        /// <summary>
        /// Creates a new PropertyValidationException relating to a single a property
        /// </summary>
        /// <param name="message">The message to assign to the exception</param>
        /// <param name="property">The property that failed validation.</param>
        /// <param name="value">Optionally value of the object/properties that caused the attribute to trigger validation error.</param>
        public PropertyValidationException(string message, string property, object value = null)
            : base(GetValidationResult(message, new string[] { property }), null, value)
        {
        }

        private static ValidationResult GetValidationResult(string message, IEnumerable<string> properties)
        {
            var vr = new ValidationResult(message, properties);
            return vr;
        }
    }
}
