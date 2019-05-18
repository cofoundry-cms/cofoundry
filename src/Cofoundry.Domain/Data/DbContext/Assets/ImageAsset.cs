using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents an image that has been uploaded to the CMS.
    /// </summary>
    public partial class ImageAsset : IUpdateAuditable
    {
        public ImageAsset()
        {
            ImageAssetGroupItems = new List<ImageAssetGroupItem>();
            ImageAssetTags = new List<ImageAssetTag>();
        }

        /// <summary>
        /// Database if of the image asset.
        /// </summary>
        public int ImageAssetId { get; set; }

        /// <summary>
        /// Original filename without an extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File name used internally for storing the file on disk (without 
        /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
        /// </summary>
        /// <remarks>
        /// For files created before file stamps were used this may
        /// contain only the image asset id.
        /// </remarks>
        public string FileNameOnDisk { get; set; }

        /// <summary>
        /// Original file extension without the leading dot.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// A random string token that can be used to verify a file request
        /// and mitigate enumeration attacks.
        /// </summary>
        public string VerificationToken { get; set; }

        /// <summary>
        /// The width of the image file in pixels.
        /// </summary>
        public int WidthInPixels { get; set; }

        /// <summary>
        /// The height of the image file in pixels.
        /// </summary>
        public int HeightInPixels { get; set; }

        /// <summary>
        /// The focal point to use when using dynamic cropping
        /// by default. 
        /// </summary>
        public ImageAnchorLocation? DefaultAnchorLocation {get; set; }

        /// <summary>
        /// The title or alt text for an image. Recommended to be up 
        /// 125 characters to accomodate screen readers.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Size of the image file on disk.
        /// </summary>
        public long FileSizeInBytes { get; set; }

        /// <summary>
        /// The date the file was last updated. Used for cache busting
        /// the asset file in web requests.
        /// </summary>
        public DateTime FileUpdateDate { get; set; }

        public virtual ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
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
