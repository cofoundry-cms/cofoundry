using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
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
        public ImageAssetFile Map(ImageAssetRenderDetails renderDetails, Stream contentStream)
        {
            if (renderDetails == null) return null;

            if (contentStream == null)
            {
                throw new ArgumentNullException(nameof(contentStream));
            }

            var image = new ImageAssetFile()
            {
                ImageAssetId = renderDetails.ImageAssetId,
                FileExtension = renderDetails.FileExtension,
                FileName = renderDetails.FileName,
                FileNameOnDisk = renderDetails.FileNameOnDisk,
                Height = renderDetails.Height,
                Width = renderDetails.Width,
                Title = renderDetails.Title,
                DefaultAnchorLocation = renderDetails.DefaultAnchorLocation,
                FileStamp = renderDetails.FileStamp,
                FileUpdateDate = renderDetails.FileUpdateDate,
                ContentStream = contentStream,
                VerificationToken = renderDetails.VerificationToken
            };

            return image;
        }
    }
}
