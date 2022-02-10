using Cofoundry.Core.Validation;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the result of a query that may have found validation errors.
    /// </summary>
    public interface IValidationQueryResult
    {
        /// <summary>
        /// True if the query discovered no errors; otherwise false.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Returns any errors encountered when runing the query.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ValidationError> GetErrors();
    }
}