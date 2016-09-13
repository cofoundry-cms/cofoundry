using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class ImageAssetTag : IEntityTag
    {
        public int ImageAssetId { get; set; }
        public int TagId { get; set; }
        public virtual ImageAsset ImageAsset { get; set; }
        public virtual Tag Tag { get; set; }

        public DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
    }
}
