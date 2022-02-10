using Cofoundry.Core.Validation;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic model to represent the result of a validation query. the
    /// result can only contain a single validation error, as typically 
    /// most queries will return after a single error is found.
    /// </summary>
    /// <inheritdoc/>
    public class ValidationQueryResult : IValidationQueryResult
    {
        public ValidationQueryResult() { }

        public ValidationQueryResult(ValidationError error)
        {
            UpdateError(error);
        }

        public virtual bool IsSuccess { get; set; }

        /// <summary>
        /// Contains the first validation error found when running the query. If no 
        /// validation errors are found then this will be <see langword="null"/>.
        /// </summary>
        public virtual ValidationError Error { get; set; }

        /// <summary>
        /// Throws a <see cref="ValidationErrorException"/> if the validation
        /// attempt was not successful.
        /// </summary>
        public virtual void ThrowIfNotSuccess()
        {
            if (IsSuccess) return;

            if (Error == null)
            {
                throw new ValidationErrorException("Invalid");
            }

            Error.Throw();
        }

        /// <summary>
        /// Updates the result with the specified error, automatically updating
        /// <see cref="IsSuccess"/> to the correct state.
        /// </summary>
        /// <param name="error"></param>
        public virtual void UpdateError(ValidationError error)
        {
            Error = error;
            IsSuccess = error == null;
        }

        /// <summary>
        /// Returns a valid new <see cref="ValidationQueryResult"/> instance
        /// without any errors.
        /// </summary>
        public static ValidationQueryResult ValidResult()
        {
            return new ValidationQueryResult(null);
        }

        public IEnumerable<ValidationError> GetErrors()
        {
            if (Error != null)
            {
                yield return Error;
            }
        }
    }
}