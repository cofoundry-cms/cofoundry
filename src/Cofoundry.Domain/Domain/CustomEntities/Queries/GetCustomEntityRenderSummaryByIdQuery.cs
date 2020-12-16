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
    /// <summary>
    /// Gets a custom entity by it's database id, returning a 
    /// general-purpose CustomEntityRenderSummary projection which
    /// includes version specific data and a deserialized data model. 
    /// The result is version-sensitive and defaults to returning published 
    /// versions only, but this behavior can be controlled by the 
    /// publishStatus query property.
    /// </summary>
    public class GetCustomEntityRenderSummaryByIdQuery : IQuery<CustomEntityRenderSummary>, IValidatableObject
    {
        /// <summary>
        /// Gets a custom entity by it's database id, returning a 
        /// general-purpose CustomEntityRenderSummary projection which
        /// includes version specific data and a deserialized data model. 
        /// The result is  version-sensitive and defaults to returning published 
        /// versions only, but this behavior can be controlled by the 
        /// publishStatus query property.
        /// </summary>
        public GetCustomEntityRenderSummaryByIdQuery() { }

        /// <summary>
        /// Gets a custom entity by it's database id, returning a 
        /// general-purpose CustomEntityRenderSummary projection which
        /// includes version specific data and a deserialized data model. 
        /// The result is  version-sensitive and defaults to returning published 
        /// versions only, but this behavior can be controlled by the 
        /// publishStatus query property.
        /// </summary>
        /// <param name="customEntityId">CustomEntityId of the custom entity to get.</param>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entity to include data for.</param>
        public GetCustomEntityRenderSummaryByIdQuery(int customEntityId, PublishStatusQuery? publishStatusQuery)
        {
            CustomEntityId = customEntityId;
            if (publishStatusQuery.HasValue)
            {
                PublishStatus = publishStatusQuery.Value;
            }
        }

        /// <summary>
        /// The database id of the custom entity to find.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Used to determine which version of the custom entities to include data for. This 
        /// defaults to Published, meaning that only published custom entities will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Use this to specify a specific version to return in the query. Mandatory when using PublishStatusQuery.SpecificVersion
        /// </summary>
        public int? CustomEntityVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return PublishStatusQueryModelValidator.ValidateVersionId(PublishStatus, CustomEntityVersionId, nameof(CustomEntityVersionId));
        }
    }
}
