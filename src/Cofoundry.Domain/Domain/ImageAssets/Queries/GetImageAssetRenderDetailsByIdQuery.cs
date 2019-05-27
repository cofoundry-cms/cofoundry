using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds an image asset by it's database id, returning an 
    /// ImageAssetRenderDetails projection if it is found, otherwise 
    /// null. An ImageAssetRenderDetails contains all the basic information 
    /// required to render out an image asset to a page, including all the 
    /// data needed to construct an asset file url.
    /// </summary>
    public class GetImageAssetRenderDetailsByIdQuery : IQuery<ImageAssetRenderDetails>
    {
        public GetImageAssetRenderDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="imageAssetId">Database id of the image asset file to get.</param>
        public GetImageAssetRenderDetailsByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        /// <summary>
        /// Database id of the image asset file to get.
        /// </summary>
        public int ImageAssetId { get; set; }
    }
}
