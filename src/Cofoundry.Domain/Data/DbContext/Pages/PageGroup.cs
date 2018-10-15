using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    [Obsolete("The page grouping system will be revised in an upcomming release.")]
    public partial class PageGroup : ICreateAuditable
    {
        public PageGroup()
        {
            PageGroupItems = new List<PageGroupItem>();
            ChildPageGroups = new List<PageGroup>();
        }

        public int PageGroupId { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> ParentGroupId { get; set; }
        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; }
        public virtual ICollection<PageGroup> ChildPageGroups { get; set; }
        public virtual PageGroup ParentPageGroup { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
