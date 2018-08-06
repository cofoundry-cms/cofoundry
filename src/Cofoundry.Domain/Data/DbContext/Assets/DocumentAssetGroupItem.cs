using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    [Obsolete("The document asset grouping system will be revised in an upcomming release.")]
    public partial class DocumentAssetGroupItem : ICreateAuditable
    {
        public int DocumentAssetId { get; set; }
        public int DocumentAssetGroupId { get; set; }
        public int Ordering { get; set; }
        public virtual DocumentAssetGroup DocumentAssetGroup { get; set; }
        public virtual DocumentAsset DocumentAsset { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
