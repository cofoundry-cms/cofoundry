using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a page by an id, projected as a PageRenderSummary which is
    /// a lighter weight projection designed for rendering to a site when the 
    /// templates, region and block data is not required. The result is 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    public class GetPageRenderSummaryByIdQuery : IQuery<PageRenderSummary>, IValidatableObject
    {
        /// <summary>
        /// Query to get a page by an id, projected as a PageRenderSummary which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        public GetPageRenderSummaryByIdQuery() { }

        /// <summary>
        /// Query to get a page by an id, projected as a PageRenderSummary which is
        /// a lighter weight projection designed for rendering to a site when the 
        /// templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        /// <param name="publishStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderSummaryByIdQuery(int pageId, PublishStatusQuery? publishStatus = null)
        {
            PageId = pageId;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
        }

        /// <summary>
        /// Database id of the page to get.
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
