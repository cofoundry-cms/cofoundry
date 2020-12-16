using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a document asset by it's database id, returning a 
    /// ImageAssetDetails projection if it is found, otherwise null.
    /// The ImageAssetDetails projection full image asset information. 
    /// This is specifically used in the admin panel and so contains audit 
    /// data and tagging information.
    /// </summary>
    public class GetImageAssetDetailsByIdQuery : IQuery<ImageAssetDetails>
    {
        public GetImageAssetDetailsByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="imageAssetId">Database id of the image asset to get.</param>
        public GetImageAssetDetailsByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        /// <summary>
        /// Database id of the image asset to get.
        /// </summary>
        public int ImageAssetId { get; set; }
    }
}
