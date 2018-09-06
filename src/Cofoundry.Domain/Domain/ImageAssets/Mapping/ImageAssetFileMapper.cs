using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetFile objects.
    /// </summary>
    public class ImageAssetFileMapper : IImageAssetFileMapper
    {
        /// <summary>
        /// Maps a ImageAssetRenderDetails (which is potentially cached) into an
        /// ImageAssetFile object. If the dbImage is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAssetRenderDetails record to map.</param>
        /// <param name="contentStream">Steam containing the file data.</param>
        public ImageAssetFile Map(ImageAssetRenderDetails cachedImage, Stream contentStream)
        {
            if (cachedImage == null) return null;

            if (contentStream == null)
            {
                throw new ArgumentNullException(nameof(contentStream));
            }

            var image = new ImageAssetFile()
            {
                ImageAssetId = cachedImage.ImageAssetId,
                Extension = cachedImage.Extension,
                FileName = cachedImage.FileName,
                Height = cachedImage.Height,
                Width = cachedImage.Width,
                Title = cachedImage.Title,
                DefaultAnchorLocation = cachedImage.DefaultAnchorLocation,
                UpdateDate = cachedImage.UpdateDate,
                ContentStream = contentStream
            };

            return image;
        }
    }
}
