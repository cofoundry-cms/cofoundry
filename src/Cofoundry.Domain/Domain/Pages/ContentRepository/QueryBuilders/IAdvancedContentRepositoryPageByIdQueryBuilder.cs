using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving page data by a unique database id.
    /// </summary>
    public interface IAdvancedContentRepositoryPageByIdQueryBuilder
        : IContentRepositoryPageByIdQueryBuilder
    {
        /// <summary>
        /// Query that returns detailed information on a page and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the PageDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        IDomainRepositoryQueryContext<PageDetails> AsDetails();

        /// <summary>
        /// Query that returns a specific version of a page (equivalent to using 
        /// PublishStatusQuery.SpecificVersion), projected as a PageRenderSummary 
        /// model, which is a lighter weight projection designed for rendering to a site 
        /// when the templates, region and block data is not required. The result is 
        /// version-sensitive and defaults to returning published versions only, but
        /// this behavior can be controlled by the publishStatus query property.
        /// </summary>
        /// <param name="pageVersionId">
        /// Use this to specify a specific version to return in the query.
        /// </param>
        IDomainRepositoryQueryContext<PageRenderSummary> AsRenderSummary(int pageVersionId);

        /// <summary>
        /// Query that returns a specific version of a page (equivalent to using 
        /// PublishStatusQuery.SpecificVersion), projected as a model that 
        /// contains all the data required to render a page, including template 
        /// data for all the content-editable regions.
        /// </summary>
        /// <param name="pageVersionId">
        /// Use this to specify a specific version to return in the query.
        /// </param>
        IDomainRepositoryQueryContext<PageRenderDetails> AsRenderDetails(int pageVersionId);
    }
}
