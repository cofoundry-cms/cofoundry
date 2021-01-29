using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving a set of image assets using a batch of 
    /// database ids. The Cofoundry.Core dictionary extensions can be 
    /// useful for ordering the results e.g. results.FilterAndOrderByKeys(ids).
    /// </summary>
    public interface IContentRepositoryImageAssetByIdRangeQueryBuilder
    {
        /// <summary>
        /// The ImageAssetRenderDetails projection contains all the basic 
        /// information required to render out an image asset to a page, 
        /// including all the data needed to construct an asset file url.
        /// </summary>
        IDomainRepositoryQueryContext<IDictionary<int, ImageAssetRenderDetails>> AsRenderDetails();
    }
}
