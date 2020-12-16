using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page directory data using a unique database id.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder
        : IContentRepositoryPageDirectoryByIdQueryBuilder
    {
        /// <summary>
        /// The PageDirectoryNode projection represents a directory in a 
        /// hierarchy of tree nodes, containing parent and child tree node 
        /// navigations properties.
        /// </summary>
        /// <returns></returns>
        IDomainRepositoryQueryContext<PageDirectoryNode> AsNode();
    }
}
