using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets file information about an image asset including
    /// stream access to the file itself.
    /// </summary>
    public class GetImageAssetFileByIdQuery : IQuery<ImageAssetFile>
    {
        public GetImageAssetFileByIdQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="imageAssetId">Database id of the image asset file to get.</param>
        public GetImageAssetFileByIdQuery(int imageAssetId)
        {
            ImageAssetId = imageAssetId;
        }

        /// <summary>
        /// Database id of the image asset file to get.
        /// </summary>
        public int ImageAssetId { get; set; }
    }
}
