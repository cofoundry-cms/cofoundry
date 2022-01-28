using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic model to represent the result of a validation query. the
    /// result can only contain a single validation error, as typically 
    /// most queries will return after a single error is found.
    /// </summary>
    public class ValidationQueryResult
    {
        public ValidationQueryResult() { }

        public ValidationQueryResult(ValidationError error)
        {
            Error = error;
            IsSuccess = error == null;
        }

        /// <summary>
        /// True if the query discovered no errors; otherwise false.
        /// </summary>
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
        /// Returns a valid new <see cref="ValidationQueryResult"/> instance
        /// without any errors.
        /// </summary>
        public static ValidationQueryResult ValidResult()
        {
            return new ValidationQueryResult(null);
        }
    }
}