using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Core.Validation
{
    /// <summary>
    /// Represents a single error when validating an object. Can be thrown by 
    /// wrapping the error in <see cref="ValidationErrorException"/>.
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
                Properties = new string[] { property };
            }
        }

        /// <summary>
        /// Zero or more properties that the error message applies to.
        /// </summary>
        public ICollection<string> Properties { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Client-friendly text describing the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional alphanumeric code representing the error that can be detected by the
        /// client to use in conditional UI flow. Errors codes are typically lowercase and 
        /// use a dash-separated namespacing convention e.g. "cf-my-entity-example-condition.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// A factory function to use when throwing the error, allowing you to specify a
        /// more specific exception to throw when <see cref="Throw"/> is called. If <see langword="null"/>
        /// then <see cref="Throw"/> will throw an <see cref="ValidationErrorException"/>.
        /// </summary>
        public Func<ValidationError, ValidationErrorException> ExceptionFactory { get; set; }

        /// <summary>
        /// Throws the error as a <see cref="ValidationErrorException"/>, or a more specific
        /// excpetion type is one is configured in the constructor.
        /// </summary>
        [DoesNotReturn]
        public void Throw()
        {
            if (ExceptionFactory != null)
            {
                throw ExceptionFactory(this);
            }

            throw new ValidationErrorException(this);
        }
    }
}
