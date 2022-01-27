using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    [Obsolete("The page grouping system will be revised in an upcomming release.")]
    public class PageGroup : ICreateAuditable
    {
        public int PageGroupId { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> ParentGroupId { get; set; }
        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; } = new List<PageGroupItem>();
        public virtual ICollection<PageGroup> ChildPageGroups { get; set; } = new List<PageGroup>();
        public virtual PageGroup ParentPageGroup { get; set; }

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }
    }
}