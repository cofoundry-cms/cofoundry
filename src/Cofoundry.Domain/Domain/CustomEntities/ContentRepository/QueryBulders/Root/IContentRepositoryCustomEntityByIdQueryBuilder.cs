using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving custom entity data using its unique database id.
    /// </summary>
    public interface IContentRepositoryCustomEntityByIdQueryBuilder
    {
        /// <summary>
        /// Queries a custom entity by it's database id, returning a 
        /// general-purpose CustomEntityRenderSummary projection which
        /// includes version specific data and a deserialized data model. 
        /// The result is version-sensitive, use the publishStatusQuery parameter
        /// to control the version returned.
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entity to include data for.</param>
        IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(PublishStatusQuery publishStatusQuery);

        /// <summary>
        /// Queries a custom entity by it's database id, returning a 
        /// general-purpose CustomEntityRenderSummary projection which
        /// includes version specific data and a deserialized data model. 
        /// The result is version-sensitive and defaults to returning published 
        /// versions only, but you can use the overload with the publishStatusQuery 
        /// parameter to control this behavior.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary();

        /// <summary>
        /// Queries a custom entity by it's database id, projected as a
        /// CustomEntityRenderDetails, which contains all data for rendering a specific 
        /// version of a custom entity out to a page, including template data for all the 
        /// content-editable page regions. This projection is specific to a particular 
        /// version which may not always be the latest (depending on the query), and to a 
        /// specific page. Although often you may only have one custom entity page, it is 
        /// possible to have multiple.
        /// </summary>
        /// <param name="pageId">
        /// PageId used to determine which page to include data for. Although often you
        /// may only have one custom entity page, it is possible to have multiple. If a
        /// page with the specified id cannot be found then no page region data will be 
        /// included in the returned object.
        /// </param>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entity to include data for.</param>
        IDomainRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, PublishStatusQuery? publishStatusQuery = null);
    }
}
