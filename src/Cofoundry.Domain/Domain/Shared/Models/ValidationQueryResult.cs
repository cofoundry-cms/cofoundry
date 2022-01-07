using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic model to represent the result of a validation query.
    /// </summary>
    public class ValidationQueryResult
    {
        public ValidationQueryResult() { }

        public ValidationQueryResult(IEnumerable<ValidationError> errors)
            : this(errors.ToArray())
        {
        }

        public ValidationQueryResult(ICollection<ValidationError> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            Errors = errors;
            IsValid = !Errors.Any();
        }

        /// <summary>
        /// True if the query discovered no errors; otherwise false.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Collection containing any validation errors discovered when executing 
        /// the query. If no errors are found then this collection will be empty.
        /// </summary>
        public ICollection<ValidationError> Errors { get; set; }
    }
}