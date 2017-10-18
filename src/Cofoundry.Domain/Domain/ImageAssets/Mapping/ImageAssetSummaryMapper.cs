using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetDetails objects.
    /// </summary>
    public class ImageAssetSummaryMapper : IImageAssetSummaryMapper
    {
        private readonly IAuditDataMapper _auditDataMapper;

        public ImageAssetSummaryMapper(
            IAuditDataMapper auditDataMapper
            )
        {
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// Maps an EF ImageAsset record from the db into a ImageAssetDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAsset record from the database.</param>
        public ImageAssetSummary Map(ImageAsset dbImage)
        {
            if (dbImage == null) return null;

            var image = new ImageAssetSummary();
            Map(image, dbImage);

            return image;
        }

        /// <summary>
        /// Used internally to map a model that inherits from ImageAssetSummary. 
        /// </summary>
        public ImageAssetSummary Map<TModel>(TModel image, ImageAsset dbImage)
            where TModel : ImageAssetSummary
        {
            image.AuditData = _auditDataMapper.MapUpdateAuditData(dbImage);
            image.ImageAssetId = dbImage.ImageAssetId;
            image.Extension = dbImage.Extension;
            image.FileName = dbImage.FileName;
            image.FileSizeInBytes = dbImage.FileSize;
            image.Height = dbImage.Height;
            image.Width = dbImage.Width;
            image.Title = dbImage.FileDescription;
            image.DefaultAnchorLocation = dbImage.DefaultAnchorLocation;
            image.Tags = dbImage
                .ImageAssetTags
                .Select(t => t.Tag.TagText)
                .OrderBy(t => t)
                .ToList();

            return image;
        }
    }
}
