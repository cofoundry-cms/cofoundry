using System;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Abstraction over access rules associated with different entity types.
    /// </summary>
    public interface IEntityAccessRule : ICreateAuditable
    {
        /// <summary>
        /// Unique 3 character code representing the <see cref="UserArea"/> to
        /// restrict access to.
        /// </summary>
        string UserAreaCode { get; set; }

        /// <summary>
        /// The <see cref="UserArea"/> to restrict access to.
        /// </summary>
        UserArea UserArea { get; set; }

        /// <summary>
        /// The optional id of the <see cref="Role"/> that this rule restricts page 
        /// access to. The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        int? RoleId { get; set; }

        /// <summary>
        /// The optional <see cref="Role"/> that this rule restricts page 
        /// access to. The role must belong to the user area defined by <see cref="UserAreaCode"/>.
        /// </summary>
        Role Role { get; set; }

        /// <summary>
        /// 3 letter code representing the action that should be taken when a user
        /// does not meet the criteria of the rule. This is mapped to the
        /// <see cref="Cofoundry.Domain.RouteAccessRuleViolationAction"/> enum.
        /// </summary>
        int RouteAccessRuleViolationActionId { get; set; }
    }
}
