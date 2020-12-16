using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a folder in the dynamic web page heirarchy. There is always a 
    /// single root directory.
    /// </summary>
    public partial class PageDirectory : ICreateAuditable
    {
        public PageDirectory()
        {
            Pages = new List<Page>();
            ChildPageDirectories = new List<PageDirectory>();
            PageDirectoryLocales = new List<PageDirectoryLocale>();
        }

        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent directory. This can only be null for the 
        /// root directory.
        /// </summary>
        public int? ParentPageDirectoryId { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
        public string UrlPath { get; set; }

        public virtual ICollection<Page> Pages { get; set; }

        public virtual ICollection<PageDirectory> ChildPageDirectories { get; set; }

        public virtual PageDirectory ParentPageDirectory { get; set; }

        public virtual ICollection<PageDirectoryLocale> PageDirectoryLocales { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        #endregion
    }
}
