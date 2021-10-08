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

        /// <summary>
        /// Unique 3 character code representing the <see cref="UserArea"/> to
        /// restrict access to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The <see cref="UserArea"/> to restrict access to.
        /// </summary>
        public virtual UserArea UserArea { get; set; }

        /// <summary>
        /// The optional id of the <see cref="Role"/> that this rule restricts page 
        /// access to. The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// The optional <see cref="Role"/> that this rule restricts page 
        /// access to. The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// 3 letter code representing the action that should be taken when a user
        /// does not meet the criteria of the rule. This is mapped to the
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
