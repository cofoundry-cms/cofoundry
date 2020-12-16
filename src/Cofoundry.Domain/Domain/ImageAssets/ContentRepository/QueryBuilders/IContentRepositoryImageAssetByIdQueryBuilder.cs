using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving image asset data for a unique database id.
    /// </summary>
    public interface IContentRepositoryImageAssetByIdQueryBuilder
    {
        /// <summary>
        /// The ImageAssetRenderDetails projection contains all the basic 
        /// information required to render out an image asset to a page, 
        /// including all the data needed to construct an asset file url.
        /// </summary>
        IDomainRepositoryQueryContext<ImageAssetRenderDetails> AsRenderDetails();
    }
}
