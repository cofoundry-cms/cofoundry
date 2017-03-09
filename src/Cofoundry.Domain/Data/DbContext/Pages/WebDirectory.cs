using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a folder in the dynamic web page heirarchy. There is always a 
    /// single root web directory.
    /// </summary>
    public partial class WebDirectory : ICreateAuditable
    {
        public WebDirectory()
        {
            Pages = new List<Page>();
            ChildWebDirectories = new List<WebDirectory>();
            WebDirectoryLocales = new List<WebDirectoryLocale>();
        }

        /// <summary>
        /// Database id of the web directory.
        /// </summary>
        public int WebDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent directory. This can only be null for the 
        /// root web directory.
        /// </summary>
        public int? ParentWebDirectoryId { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
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
