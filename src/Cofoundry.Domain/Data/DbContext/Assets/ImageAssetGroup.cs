using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    [Obsolete("The image asset grouping system will be revised in an upcomming release.")]
    public partial class ImageAssetGroup : ICreateAuditable
    {
        public ImageAssetGroup()
        {
            ImageAssetGroupItems = new List<ImageAssetGroupItem>();
            ChildImageAssetGroups = new List<ImageAssetGroup>();
        }

        public int ImageAssetGroupId { get; set; }
        public string GroupName { get; set; }
        public int? ParentImageAssetGroupId { get; set; }
        public virtual ICollection<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }
        public virtual ICollection<ImageAssetGroup> ChildImageAssetGroups { get; set; }
        public virtual ImageAssetGroup ParentImageAssetGroup { get; set; }

        #region ICreateAuditable

        public DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
