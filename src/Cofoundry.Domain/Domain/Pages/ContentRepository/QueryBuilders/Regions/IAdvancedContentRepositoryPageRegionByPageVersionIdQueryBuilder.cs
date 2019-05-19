using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retieving regions and blocks for a specific verison of a page.
    /// </summary>
    public interface IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder
    {
        /// <summary>
        /// Returns a collection of content managed regions with
        /// block data for a specific version of a page.
        /// </summary>
        Task<ICollection<PageRegionDetails>> AsDetailsAsync();
    }
}
