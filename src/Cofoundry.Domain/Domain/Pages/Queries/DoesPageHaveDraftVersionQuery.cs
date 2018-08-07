using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if a page has a draft version of not. A page can only have one draft
    /// version at a time.
    /// </summary>
    public class DoesPageHaveDraftVersionQuery : IQuery<bool>
    {
        /// <summary>
        /// Determines if a page has a draft version of not. A page can only have one draft
        /// version at a time.
        /// </summary>
        public DoesPageHaveDraftVersionQuery() { }

        /// <summary>
        /// Determines if a page has a draft version of not. A page can only have one draft
        /// version at a time.
        /// </summary>
        /// <param name="pageId">Id of the page to check.</param>
        public DoesPageHaveDraftVersionQuery(int pageId)
        {
            PageId = pageId;
        }

        /// <summary>
        /// Id of the page to check.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageId { get; set; }
    }
}
