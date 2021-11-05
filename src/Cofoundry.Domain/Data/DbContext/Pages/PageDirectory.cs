using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Represents a folder in the dynamic web page heirarchy. There is always a 
    /// single root directory.
    /// </summary>
    /// <inheritdoc/>
    public partial class PageDirectory : IEntityAccessRestrictable<PageDirectoryAccessRule>, ICreateAuditable
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Id of the parent <see cref="PageDirectory"/>. This can only be null for the 
        /// root directory.
        /// </summary>
        public int? ParentPageDirectoryId { get; set; }

        /// <summary>
        /// The parent <see cref="PageDirectory"/>. This can only be null for the 
        /// root directory.
        /// </summary>
        public virtual PageDirectory ParentPageDirectory { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url slug used to create a path for this directory. Should not
        /// contain any slashes, just alpha-numerical with dashes.
        /// </summary>
        public string UrlPath { get; set; }

        public int AccessRuleViolationActionId { get; set; }

        public string UserAreaCodeForLoginRedirect { get; set; }

        public virtual UserArea UserAreaForLoginRedirect { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        /// <summary>
        /// A directory can have zero or more pages. A <see cref="Page"/>
        /// without a <see cref="Page.UrlPath"/> is treated as the root or
        /// index page in a directory.
        /// </summary>
        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

        /// <summary>
        /// A directory can have zero or more child directories, creating
        /// a heirachical structure.
        /// </summary>
        public virtual ICollection<PageDirectory> ChildPageDirectories { get; set; } = new List<PageDirectory>();

        public virtual ICollection<PageDirectoryLocale> PageDirectoryLocales { get; set; } = new List<PageDirectoryLocale>();

        /// <summary>
        /// <para>
        /// Access rules are used to restrict access to a website resource to users
        /// fulfilling certain criteria such as a specific user area or role. Page
        /// directory access rules are used to define the rules at a <see cref="PageDirectory"/> 
        /// level. These rules are inherited by child directories and pages.
        /// </para>
        /// <para>
        /// Note that access rules do not apply to users from the Cofoundry Admin user
        /// area. They aren't intended to be used to restrict editor access in the admin UI 
        /// but instead are used to restrict public access to website pages and routes.
        /// </para>
        /// </summary>
        public virtual ICollection<PageDirectoryAccessRule> AccessRules { get; set; } = new List<PageDirectoryAccessRule>();

        /// <summary>
        /// Records from the <see cref="PageDirectoryClosure"/> table where this directory is the descendant
        /// in the relationship i.e. this can be used to reference ancestors. The records will 
        /// include a self-referencing node. The <see cref="PageDirectoryClosure"/> table is automatically 
        /// generated and this collection should not be amended manually.
        /// </summary>
        public virtual ICollection<PageDirectoryClosure> AncestorPageDirectories { get; set; }

        /// <summary>
        /// Records from the <see cref="PageDirectoryClosure"/> table where this directory is the ancestor
        /// in the relationship i.e. this can be used to reference descendants. The records will 
        /// include a self-referencing node. The <see cref="PageDirectoryClosure"/> table is automatically 
        /// generated and this collection should not be amended manually.
        /// </summary>
        public virtual ICollection<PageDirectoryClosure> DescendantPageDirectories { get; set; }

        /// <summary>
        /// Information about the full directory path and it's position in the directory 
        /// heirachy. This table is automatically updated whenever changes are made to the page 
        /// directory heirarchy and should be treated as read-only.
        /// </summary>
        public virtual PageDirectoryPath PageDirectoryPath { get; set; }

        public int GetId()
        {
            return PageDirectoryId;
        }
    }
}
