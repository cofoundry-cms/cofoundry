using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page data by a url path.
    /// </summary>
    public interface IContentRepositoryPageByPathQueryBuilder
    {
        /// <summary>
        /// A query that attempts to find a matching page route using the supplied path. The path
        /// has to be an absolute match, i.e. the query does not try and find a fall-back 
        /// similar route.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        IDomainRepositoryQueryContext<PageRoutingInfo> AsRoutingInfo(GetPageRoutingInfoByPathQuery query);
    }
}
