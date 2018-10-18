using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a projection of a page that contains the data required to render a page, including template 
    /// data for all the content-editable regions.
    /// </summary>
    public class GetPageRenderDetailsByIdQuery : IQuery<PageRenderDetails>, IValidatableObject
    {
        /// <summary>
        /// Gets a projection of a page that contains the data required to render a page, including template 
        /// data for all the content-editable regions.
        /// </summary>
        public GetPageRenderDetailsByIdQuery() { }

        /// <summary>
        /// Gets a projection of a page that contains the data required to render a page, including template 
        /// data for all the content-editable regions.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderDetailsByIdQuery(int pageId, PublishStatusQuery? publishStatus = null)
        {
            PageId = pageId;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        /// <summary>
        /// PageId of the page to get.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        /// <summary>
        /// Used to determine which version of the page to include data for. This 
        /// defaults to Published, meaning that only published pages will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Optional id of a specific page version to get. Can only be provided
        /// if PublishStatusQuery is set to PublishStatusQuery.SpecificVersion.
        /// </summary>
        public int? PageVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return PublishStatusQueryModelValidator.ValidateVersionId(PublishStatus, PageVersionId, nameof(PageVersionId));
        }
    }
}
