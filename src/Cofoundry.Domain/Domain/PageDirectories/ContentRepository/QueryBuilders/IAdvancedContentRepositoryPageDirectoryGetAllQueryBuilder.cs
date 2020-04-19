using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for returning all page directories.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder
        : IContentRepositoryPageDirectoryGetAllQueryBuilder
    {
        /// <summary>
        /// Returns a complete tree of page directory nodes, starting
        /// with the root page directory as a single node.
        /// </summary>
        Task<PageDirectoryNode> AsTreeAsync();
    }
}
