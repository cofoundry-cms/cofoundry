using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetRenderDetails objects.
    /// </summary>
    public class ImageAssetRenderDetailsMapper : IImageAssetRenderDetailsMapper
    {
        /// <summary>
        /// Maps an EF ImageAsset record from the db into a ImageAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAsset record from the database.</param>
        public ImageAssetRenderDetails Map(ImageAsset dbImage)
        {
            if (dbImage == null) return null;

            var image = new ImageAssetRenderDetails();

            image.ImageAssetId = dbImage.ImageAssetId;
            image.Extension = dbImage.Extension;
            image.FileName = dbImage.FileName;
            image.Height = dbImage.Height;
            image.Width = dbImage.Width;
            image.Title = dbImage.FileDescription;
            image.DefaultAnchorLocation = dbImage.DefaultAnchorLocation;
            image.UpdateDate = dbImage.UpdateDate;

            return image;
        }
    }
}
