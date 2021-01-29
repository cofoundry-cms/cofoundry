using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for template regions data in a custom entity page.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityPageRegionsRepository
    {
        /// <summary>
        /// Queries and commands for block data on a custom entity page.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityPageBlocksRepository Blocks();
    }
}
