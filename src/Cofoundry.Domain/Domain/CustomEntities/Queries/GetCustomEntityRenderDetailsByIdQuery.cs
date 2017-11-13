using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class GetCustomEntityRenderDetailsByIdQuery : IQuery<CustomEntityRenderDetails>, IValidatableObject
    {
        public GetCustomEntityRenderDetailsByIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified parameters.
        /// </summary>
        /// <param name="customEntityId">CustomEntityId of the custom entity to get.</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetCustomEntityRenderDetailsByIdQuery(int customEntityId, PublishStatusQuery? publishStatus = null)
        {
            CustomEntityId = customEntityId;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        public PublishStatusQuery PublishStatus { get; set; }

        public int? CustomEntityVersionId { get; set; }

        [Required]
        [PositiveInteger]
        public int PageTemplateId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PublishStatus == PublishStatusQuery.SpecificVersion && (!CustomEntityVersionId.HasValue || CustomEntityVersionId < 1))
            {
                yield return new ValidationResult("Value cannot be null if PublishStatusQuery.SpecificVersion is specified", new string[] { "CustomEntityVersionId" });
            }
            else if (PublishStatus != PublishStatusQuery.SpecificVersion && CustomEntityVersionId.HasValue)
            {
                yield return new ValidationResult("Value should be null if PublishStatusQuery.SpecificVersion is not specified", new string[] { "CustomEntityVersionId" });
            }
        }

    }
}
