using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class WebDirectory : ICreateAuditable
    {
        public WebDirectory()
        {
            Pages = new List<Page>();
            ChildWebDirectories = new List<WebDirectory>();
            WebDirectoryLocales = new List<WebDirectoryLocale>();
        }

        public int WebDirectoryId { get; set; }
        public Nullable<int> ParentWebDirectoryId { get; set; }
        public string Name { get; set; }
        public string UrlPath { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
        public virtual ICollection<WebDirectory> ChildWebDirectories { get; set; }
        public virtual WebDirectory ParentWebDirectory { get; set; }
        public virtual ICollection<WebDirectoryLocale> WebDirectoryLocales { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
