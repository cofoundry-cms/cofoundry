using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for returning all page directories.
    /// </summary>
    public interface IContentRepositoryPageDirectoryGetAllQueryBuilder
    {
        /// <summary>
        /// The PageDirectoryRoute projection is used in dynamic page routing and is designed to
        /// be lightweight and cacheable. The results of this query are cached by default.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<PageDirectoryRoute>> AsRoutes();
    }
}
