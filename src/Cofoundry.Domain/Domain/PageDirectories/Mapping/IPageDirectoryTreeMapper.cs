using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A mapper for mapping a tree structure of PageDirectoryNode objects.
    /// </summary>
    public interface IPageDirectoryTreeMapper
    {
        /// <summary>
        /// Maps a collection of projected EF PageDirectoryTreeNodeQueryModel records from the db 
        /// into a tree of PageDirectoryNode instances with a single root node
        /// object.
        /// </summary>
        /// <param name="dbPageDirectories">PageDirectoryTreeNodeQueryModel records from the database.</param>
        PageDirectoryNode Map(IReadOnlyCollection<PageDirectoryTreeNodeQueryModel> dbPageDirectories);
    }
}
