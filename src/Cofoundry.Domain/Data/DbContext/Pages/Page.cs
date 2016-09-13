using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Page : ICreateAuditable
    {
        public Page()
        {
            PageGroupItems = new List<PageGroupItem>();
            PageTags = new List<PageTag>();
            PageVersions = new List<PageVersion>();
        }

        public int PageId { get; set; }
        public int WebDirectoryId { get; set; }
        public Nullable<int> LocaleId { get; set; }
        public string UrlPath { get; set; }
        public int PageTypeId { get; set; }
        public bool IsDeleted { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public virtual Locale Locale { get; set; }
        public virtual WebDirectory WebDirectory { get; set; }
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; }
        public virtual ICollection<PageTag> PageTags { get; set; }
        public virtual ICollection<PageVersion> PageVersions { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
