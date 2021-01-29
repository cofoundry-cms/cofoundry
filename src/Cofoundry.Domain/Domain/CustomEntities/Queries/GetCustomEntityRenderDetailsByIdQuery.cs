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
    /// Query to retreive a custom entity by it's database id, projected as a
    /// CustomEntityRenderDetails, which contains all data for rendering a specific 
    /// version of a custom entity out to a page, including template data for all the 
    /// content-editable page regions. This projection is specific to a particular 
    /// version which may not always be the latest (depending on the query), and to a 
    /// specific page. Although often you may only have one custom entity page, it is 
    /// possible to have multiple.
    /// </summary>
    public class GetCustomEntityRenderDetailsByIdQuery : IQuery<CustomEntityRenderDetails>, IValidatableObject
    {
        /// <summary>
        /// Database id of the custom entity to get.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Used to determine which version of the custom entity to include data for. This 
        /// defaults to Published, meaning that only published custom entities will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Use this to specify a specific version to return in the query. Mandatory when using PublishStatusQuery.SpecificVersion
        /// </summary>
        public int? CustomEntityVersionId { get; set; }

        /// <summary>
        /// PageId to use to determine which page to include data for. Although often you
        /// may only have one custom entity page, it is possible to have multiple. If a
        /// page with the specified id cannot be found then no page region data will be 
        /// included in the returned object.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return PublishStatusQueryModelValidator.ValidateVersionId(PublishStatus, CustomEntityVersionId, nameof(CustomEntityVersionId));
        }

    }
}
