using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class ImageAsset : IUpdateAuditable
    {
        public ImageAsset()
        {
            ImageAssetGroupItems = new List<ImageAssetGroupItem>();
            ImageAssetTags = new List<ImageAssetTag>();
        }

        public int ImageAssetId { get; set; }

        /// <summary>
        /// Original filename without an extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Original file extension without the leading dot.
        /// </summary>
        public string Extension { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// The default Anchor Location when using dynamic cropping
        /// </summary>
        public ImageAnchorLocation? DefaultAnchorLocation {get; set; }

        public string FileDescription { get; set; }

        /// <summary>
        /// Size of the image file on disk. Assumes files will never be more than 2gb, which i think is reasonable for an image.
        /// </summary>
        public int FileSize { get; set; }
        
        public bool IsDeleted { get; set; }

        public virtual ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }
        public virtual ICollection<ImageAssetTag> ImageAssetTags { get; set; }

        #region IUpdateAuditable

        public DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        public DateTime UpdateDate { get; set; }
        public int UpdaterId { get; set; }
        public virtual User Updater { get; set; }

        #endregion
    }
}
