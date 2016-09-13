using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class DocumentAsset : IUpdateAuditable
    {
        public DocumentAsset()
        {
            DocumentAssetGroupItems = new List<DocumentAssetGroupItem>();
            DocumentAssetTags = new List<DocumentAssetTag>();
        }

        public int DocumentAssetId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string ContentType { get; set; }
        public bool IsDeleted { get; set; }

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
