using System;

namespace Cofoundry.Domain.Data
{
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
    public class PageDirectoryAccessRule : IEntityAccessRule
    {
        /// <summary>
        /// Database primary key.
        /// </summary>
        public int PageDirectoryAccessRuleId { get; set; }

        /// <summary>
        /// Id of the <see cref="PageDirectory"/> that this rule controls access 
        /// to, as well as any child directories or pages.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// <see cref="PageDirectory"/> that this rule controls access to,
        /// as well as any child directories or pages.
        /// </summary>
        public virtual PageDirectory PageDirectory { get; set; }

        /// <summary>
        /// Unique 6 character code representing the <see cref="UserArea"/> to
        /// restrict access to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The <see cref="UserArea"/> to restrict access to.
        /// </summary>
        public virtual UserArea UserArea { get; set; }

        /// <summary>
        /// The optional id of the <see cref="Role"/> that this rule restricts 
        /// access to. The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// The optional <see cref="Role"/> that this rule restricts access to. 
        /// The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// 3 letter code representing the action that should be taken when a user
        /// does not meet the criteria of the rule. This maps to the
        /// <see cref="Cofoundry.Domain.RouteAccessRuleViolationAction"/> enum.
        /// </summary>
        public int RouteAccessRuleViolationActionId { get; set; }

        /// <summary>
        /// Date and time at which the rule was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The database id of the <see cref="User"/> that created the rule.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// The <see cref="User"/> that created the rule.
        /// </summary>
        public virtual User Creator { get; set; }
    }
}
