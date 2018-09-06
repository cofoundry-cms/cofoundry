using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns detailed information on a page and it's latest version. This 
    /// query is primarily used in the admin area because it is not version-specific
    /// and the PageDetails projection includes audit data and other additional 
    /// information that should normally be hidden from a customer facing app.
    /// </summary>
    public class GetPageDetailsByIdQuery : IQuery<PageDetails>
    {
        /// <summary>
        /// Returns detailed information on a page and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the PageDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        public GetPageDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Returns detailed information on a page and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the PageDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        /// <param name="pageId">Database id of the page to get.</param>
        public GetPageDetailsByIdQuery(int pageId)
        {
            PageId = pageId;
        }

        /// <summary>
        /// Database id of the page to get.
        /// </summary>
        public int PageId { get; set; }
    }
}
