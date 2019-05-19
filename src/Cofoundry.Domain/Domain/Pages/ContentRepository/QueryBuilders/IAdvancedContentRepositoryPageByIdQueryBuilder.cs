using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving page data by a unique database id.
    /// </summary>
    public interface IAdvancedContentRepositoryPageByIdQueryBuilder
        : IContentRepositoryPageByIdQueryBuilder
    {
        /// <summary>
        /// Returns detailed information on a page and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the PageDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        Task<PageDetails> AsPageDetailsAsync();
    }
}
