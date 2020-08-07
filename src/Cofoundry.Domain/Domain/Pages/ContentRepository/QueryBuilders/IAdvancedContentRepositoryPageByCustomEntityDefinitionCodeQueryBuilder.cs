using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving custom entity page data for a single custom 
    /// entity type.
    /// </summary>
    public interface IAdvancedContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder
    {
        /// <summary>
        /// Query that returns all page routes for custom entity details pages for a 
        /// specific custom entity definition. Typically there would only
        /// be a single page for a custom entity (e.g. blog post details)
        /// but it is possible to have multiple.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes();
    }
}
