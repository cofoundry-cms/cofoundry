using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the ImageAsset entity.
    /// </summary>
    public interface IContentRepositoryImageAssetRepository
    {
        #region queries

        /// <summary>
        /// Retrieve an image asset by a unique database id.
        /// </summary>
        /// <param name="imageAssetId">ImageAssetId of the image to get.</param>
        IContentRepositoryImageAssetByIdQueryBuilder GetById(int imageAssetId);

        /// <summary>
        /// Retrieve a set of image assets using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="imageAssetIds">Range of ImageAssetIds of the images to get.</param>
        IContentRepositoryImageAssetByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> imageAssetIds);

        /// <summary>
        /// Search for image assets, returning paged lists of data.
        /// </summary>
        IContentRepositoryImageAssetSearchQueryBuilder Search();

        #endregion
    }
}
