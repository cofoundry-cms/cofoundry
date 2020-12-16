using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetDetails objects.
    /// </summary>
    public class ImageAssetDetailsMapper : IImageAssetDetailsMapper
    {
        private readonly ImageAssetSummaryMapper _imageAssetSummaryMapper;

        public ImageAssetDetailsMapper(
            IAuditDataMapper auditDataMapper,
            IImageAssetRouteLibrary imageAssetRouteLibrary
            )
        {
            _imageAssetSummaryMapper = new ImageAssetSummaryMapper(auditDataMapper, imageAssetRouteLibrary);
        }

        /// <summary>
        /// Maps an EF ImageAsset record from the db into a ImageAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAsset record from the database.</param>
        public ImageAssetDetails Map(ImageAsset dbImage)
        {
            if (dbImage == null) return null;

            var image = new ImageAssetDetails();
            _imageAssetSummaryMapper.Map(image, dbImage);
            image.FileUpdateDate = dbImage.FileUpdateDate;
            return image;
        }
    }
}
