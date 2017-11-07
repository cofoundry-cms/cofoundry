using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a page object that contains the data required to render a page, including template 
    /// data for all the content-editable regions.
    /// </summary>
    public class GetPageRenderDetailsByIdQuery : IQuery<PageRenderDetails>, IValidatableObject
    {
        public GetPageRenderDetailsByIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified parameters.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        /// <param name="workFlowStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderDetailsByIdQuery(int pageId, PublishStatusQuery? workFlowStatus = null)
        {
            PageId = pageId;
            if (workFlowStatus.HasValue)
            {
                PublishStatus = workFlowStatus.Value;
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
        /// defaults to Latest, meaning that a draft page will be returned ahead of a
        /// published version of the page.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }

        /// <summary>
        /// Optional id of a specific page version to get. Can only be provided
        /// if PublishStatusQuery is set to PublishStatusQuery.SpecificVersion.
        /// </summary>
        public int? PageVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PublishStatus == PublishStatusQuery.SpecificVersion && (!PageVersionId.HasValue || PageVersionId < 1))
            {
                yield return new ValidationResult("Value cannot be null if PublishStatusQuery.SpecificVersion is specified", new string[] { nameof(PageVersionId) });
            }
            else if (PublishStatus != PublishStatusQuery.SpecificVersion && PageVersionId.HasValue)
            {
                yield return new ValidationResult("Value should be null if PublishStatusQuery.SpecificVersion is not specified", new string[] { nameof(PageVersionId) });
            }
        }
    }
}
