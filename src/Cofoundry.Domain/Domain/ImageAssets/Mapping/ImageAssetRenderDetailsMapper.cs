using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetRenderDetails objects.
    /// </summary>
    public class ImageAssetRenderDetailsMapper : IImageAssetRenderDetailsMapper
    {
        private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

        public ImageAssetRenderDetailsMapper(
            IImageAssetRouteLibrary imageAssetRouteLibrary
            )
        {
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
        }

        /// <summary>
        /// Maps an EF ImageAsset record from the db into a ImageAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAsset record from the database.</param>
        public ImageAssetRenderDetails Map(ImageAsset dbImage)
        {
            if (dbImage == null) return null;

            var image = new ImageAssetRenderDetails()
            {
                ImageAssetId = dbImage.ImageAssetId,
                FileExtension = dbImage.FileExtension,
                FileName = dbImage.FileName,
                FileNameOnDisk = dbImage.FileNameOnDisk,
                Height = dbImage.HeightInPixels,
                Width = dbImage.WidthInPixels,
                Title = dbImage.Title,
                DefaultAnchorLocation = dbImage.DefaultAnchorLocation,
                FileUpdateDate = dbImage.FileUpdateDate,
                VerificationToken = dbImage.VerificationToken
            };

            image.FileStamp = AssetFileStampHelper.ToFileStamp(dbImage.FileUpdateDate);
            image.Url = _imageAssetRouteLibrary.ImageAsset(image);

            return image;
        }
    }
}
