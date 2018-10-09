using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a non-image file that has been uploaded to the 
    /// CMS. The name could be misleading here as any file type except
    /// images are supported, but at least it is less ambigous than the 
    /// term 'file'.
    /// </summary>
    public partial class DocumentAsset : IUpdateAuditable
    {
        public DocumentAsset()
        {
            DocumentAssetGroupItems = new List<DocumentAssetGroupItem>();
            DocumentAssetTags = new List<DocumentAssetTag>();
        }

        /// <summary>
        /// Database id of the document asset.
        /// </summary>
        public int DocumentAssetId { get; set; }

        /// <summary>
        /// The filename is taken from the title property
        /// and cleaned to remove invalid characters.
        /// </summary>
        public string FileName { get; set; }

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
        /// File name used internally for storing the file on disk (without 
        /// extension). This is typically in the format {ImageAssetId}-{FileStamp}.
        /// </summary>
        /// <remarks>
        /// For files created before file stamps were used this may
        /// contain only the image asset id.
        /// </remarks>
        public string FileNameOnDisk { get; set; }

        /// <summary>
        /// A short descriptive title of the document (130 characters).
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A longer description of the document in plain text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The length of the document file, in bytes.
        /// </summary>
        public long FileSizeInBytes { get; set; }

        /// <summary>
        /// The date the file was last updated. Used for cache busting
        /// the asset file in web requests.
        /// </summary>
        public DateTime FileUpdateDate { get; set; }

        public string ContentType { get; set; }

        public virtual ICollection<DocumentAssetGroupItem> DocumentAssetGroupItems { get; set; }

        public virtual ICollection<DocumentAssetTag> DocumentAssetTags { get; set; }

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
