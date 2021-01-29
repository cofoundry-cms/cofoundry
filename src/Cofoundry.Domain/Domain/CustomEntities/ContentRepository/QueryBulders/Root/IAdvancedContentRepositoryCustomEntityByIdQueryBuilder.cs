using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving custom entity data by its unique database id.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityByIdQueryBuilder
        : IContentRepositoryCustomEntityByIdQueryBuilder
    {
        /// <summary>
        /// Gets detailed information on a custom entity and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the CustomEntityDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        IDomainRepositoryQueryContext<CustomEntityDetails> AsDetails();

        /// <summary>
        /// Gets a specific version of a custom entity (equivalent to using 
        /// PublishStatusQuery.SpecificVersion), returning a 
        /// general-purpose CustomEntityRenderSummary projection which
        /// includes version specific data and a deserialized data model. 
        /// The result is version-sensitive and defaults to returning published 
        /// versions only, but this behavior can be controlled by the 
        /// publishStatusQuery parameter.
        /// </summary>
        /// <param name="customEntityVersionId">
        /// Use this to specify a specific version to return in the query.
        /// </param>
        IDomainRepositoryQueryContext<CustomEntityRenderSummary> AsRenderSummary(int customEntityVersionId);

        /// <summary>
        /// Gets a specific version of a custom entity (equivalent to using 
        /// PublishStatusQuery.SpecificVersion), projected as a
        /// CustomEntityRenderDetails, which contains all data for rendering a specific 
        /// version of a custom entity out to a page, including template data for all the 
        /// content-editable page regions. This projection is specific to a particular 
        /// version which may not always be the latest (depending on the query), and to a 
        /// specific page. Although often you may only have one custom entity page, it is 
        /// possible to have multiple.
        /// </summary>
        /// <param name="pageId">
        /// PageId to use to determine which page to include data for. Although often you
        /// may only have one custom entity page, it is possible to have multiple. If a
        /// page with the specified id cannot be found then no page region data will be 
        /// included in the returned object.
        /// </param>
        /// <param name="customEntityVersionId">
        /// Use this to specify a specific version to return in the query.
        /// </param>
        IDomainRepositoryQueryContext<CustomEntityRenderDetails> AsRenderDetails(int pageId, int customEntityVersionId);
    }
}
