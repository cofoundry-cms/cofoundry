using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// <para>
    /// Access rules are used to restrict access to a website resource to users
    /// fulfilling certain criteria such as a specific user area or role. Page
    /// access rules are used to define the rules at a <see cref="Page"/> level, 
    /// however rules are also inherited from the directories the page is parented to.
    /// </para>
    /// <para>
    /// Note that access rules do not apply to users from the Cofoundry Admin user
    /// area. They aren't intended to be used to restrict editor access in the admin UI 
    /// but instead are used to restrict public access to website pages and routes.
    /// </para>
    /// </summary>
    /// <inheritdoc/>
    public class PageAccessRule : IEntityAccessRule
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageAccessRuleId { get; set; }

        /// <summary>
        /// Id of the <see cref="Page"/> that this rule controls access to.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// <see cref="Page"/> that this rule controls access to.
        /// </summary>
        public virtual Page Page { get; set; }

        public string UserAreaCode { get; set; }

        public virtual UserArea UserArea { get; set; }

        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        public virtual User Creator { get; set; }

        public int GetId()
        {
            return PageAccessRuleId;
        }
    }
}