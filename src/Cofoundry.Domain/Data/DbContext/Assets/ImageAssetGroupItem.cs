using System;

namespace Cofoundry.Domain.Data
{
    [Obsolete("The image asset grouping system will be revised in an upcomming release.")]
    public class ImageAssetGroupItem : ICreateAuditable
    {
        public int ImageAssetId { get; set; }
        public int ImageAssetGroupId { get; set; }
        public int Ordering { get; set; }
        public virtual ImageAssetGroup ImageAssetGroup { get; set; }
        public virtual ImageAsset ImageAsset { get; set; }

        public DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
    }
}