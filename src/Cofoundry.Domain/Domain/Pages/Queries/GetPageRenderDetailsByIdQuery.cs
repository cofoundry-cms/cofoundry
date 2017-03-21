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
    /// data for all the content-editable sections.
    /// </summary>
    public class GetPageRenderDetailsByIdQuery : IQuery<PageRenderDetails>, IValidatableObject
    {
        public GetPageRenderDetailsByIdQuery() { }

        /// <summary>
        /// Initializes the query with the specified parameters.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        /// <param name="workFlowStatus">Used to determine which version of the page to include data for.</param>
        public GetPageRenderDetailsByIdQuery(int pageId, WorkFlowStatusQuery? workFlowStatus = null)
        {
            PageId = pageId;
            if (workFlowStatus.HasValue)
            {
                WorkFlowStatus = workFlowStatus.Value;
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
        public WorkFlowStatusQuery WorkFlowStatus { get; set; }

        /// <summary>
        /// Optional id of a specific page version to get. Can only be provided
        /// if WorkFlowStatusQuery is set to WorkFlowStatusQuery.SpecificVersion.
        /// </summary>
        public int? PageVersionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WorkFlowStatus == WorkFlowStatusQuery.SpecificVersion && (!PageVersionId.HasValue || PageVersionId < 1))
            {
                yield return new ValidationResult("Value cannot be null if WorkFlowStatusQuery.SpecificVersion is specified", new string[] { nameof(PageVersionId) });
            }
            else if (WorkFlowStatus != WorkFlowStatusQuery.SpecificVersion && PageVersionId.HasValue)
            {
                yield return new ValidationResult("Value should be null if WorkFlowStatusQuery.SpecificVersion is not specified", new string[] { nameof(PageVersionId) });
            }
        }
    }
}
