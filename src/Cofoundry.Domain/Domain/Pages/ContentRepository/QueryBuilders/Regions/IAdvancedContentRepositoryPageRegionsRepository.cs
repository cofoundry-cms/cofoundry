using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for template regions data in page.
    /// </summary>
    public interface IAdvancedContentRepositoryPageRegionsRepository
    {
        /// <summary>
        /// Retrieve regions for a specific verison of a page.
        /// </summary>
        /// <param name="pageVersionId">PageVersionId to query pages with.</param>
        IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder GetByPageVersionId(int pageVersionId);

        /// <summary>
        /// Each page region can contain one or more blocks of content. The data
        /// and rendering of each block is controlled by the page block type 
        /// assigned to it.
        /// </summary>
        IAdvancedContentRepositoryPageBlocksRepository Blocks();
    }
}
