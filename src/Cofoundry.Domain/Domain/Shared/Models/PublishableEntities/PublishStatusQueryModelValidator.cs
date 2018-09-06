using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class PublishStatusQueryModelValidator
    {
        /// <summary>
        /// Validates that a query model has a valid version id property
        /// for the provided PublishStatusQuery value. E.g. the version id
        /// should only be specified if the publish status is set to 
        /// PublishStatusQuery.SpecificVersion. Any errors are returned as 
        /// validation results, intended to be used in an IValidatableObject 
        /// implementation.
        /// </summary>
        /// <param name="publishStatusQuery">PublishStatusQuery value to check.</param>
        /// <param name="pageVersionId">Version id property to check.</param>
        /// <param name="versionIdPropertyName">The name of the version id property to return in any error messages.</param>
        public static IEnumerable<ValidationResult> ValidateVersionId(
            PublishStatusQuery publishStatusQuery, 
            int? pageVersionId, 
            string versionIdPropertyName
            )
        {
            if (publishStatusQuery == PublishStatusQuery.SpecificVersion && (!pageVersionId.HasValue || pageVersionId < 1))
            {
                yield return new ValidationResult($"Value cannot be null if {nameof(PublishStatusQuery)}.{nameof(PublishStatusQuery.SpecificVersion)} is specified", new string[] { versionIdPropertyName });
            }
            else if (publishStatusQuery != PublishStatusQuery.SpecificVersion && pageVersionId.HasValue)
            {
                yield return new ValidationResult($"Value should be null if {nameof(PublishStatusQuery)}.{nameof(PublishStatusQuery.SpecificVersion)} is not specified", new string[] { versionIdPropertyName });
            }
        }
    }
}
