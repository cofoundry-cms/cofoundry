using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageGroupItem : ICreateAuditable
    {
        public int PageId { get; set; }
        public int PageGroupId { get; set; }
        public int Ordering { get; set; }
        public virtual PageGroup PageGroup { get; set; }
        public virtual Page Page { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
