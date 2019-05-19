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
        /// Queries and commands for page version block data.
        /// </summary>
        IAdvancedContentRepositoryPageBlocksRepository Blocks();
    }
}
