using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Represents a single error when validating an object. Can be
    /// thrown using ValidationErrorException.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Represents a single error when validating an object.
        /// </summary>
        public ValidationError()
        {
        }

        /// <summary>
        /// Represents a single error when validating an object.
        /// </summary>
        /// <param name="message">Client-friendly text describing the error.</param>
        /// <param name="property">Optional property that the validation message applies to.</param>
        public ValidationError(string message, string property = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message");
            }

            Message = message;
            if (string.IsNullOrWhiteSpace(property))
            {
                Properties = new string[1] { property };
            }
        }

        /// <summary>
        /// Zero or more properties that the error message applies to.
        /// </summary>
        public ICollection<string> Properties { get; set; }

        /// <summary>
        /// Client-friendly text describing the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional alphanumeric code representing the error that can be detected by the
        /// client to use in conditional UI flow.
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
