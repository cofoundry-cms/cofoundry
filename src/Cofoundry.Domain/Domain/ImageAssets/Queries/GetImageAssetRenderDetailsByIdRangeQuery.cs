using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a range of image assets by their ids projected as a set of
    /// ImageAssetRenderDetails models. An ImageAssetRenderDetails 
    /// contains all the basic information required to render out an image asset
    /// to a page, including all the data needed to construct an asset file 
    /// url.
    /// </summary>
    public class GetImageAssetRenderDetailsByIdRangeQuery : IQuery<IDictionary<int, ImageAssetRenderDetails>>
    {
        public GetImageAssetRenderDetailsByIdRangeQuery()
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="imageAssetIds">Collection of database ids of the image assets to get.</param>
        public GetImageAssetRenderDetailsByIdRangeQuery(
            IEnumerable<int> imageAssetIds
            )
            : this(imageAssetIds?.ToList())
        {
        }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="imageAssetIds">Collection of database ids of the image assets to get.</param>
        public GetImageAssetRenderDetailsByIdRangeQuery(
            IReadOnlyCollection<int> imageAssetIds
            )
        {
            if (imageAssetIds == null) throw new ArgumentNullException(nameof(imageAssetIds));

            ImageAssetIds = imageAssetIds;
        }

        /// <summary>
        /// Collection of database ids of the image assets to get.
        /// </summary>
        [Required]
        public IReadOnlyCollection<int> ImageAssetIds { get; set; }
    }
}
