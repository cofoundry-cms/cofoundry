using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the common properties of an "access info" projection for
    /// an entity such as a Page or Page Directory.
    /// </summary>
    /// <typeparam name="TEntityAccessRuleSummary">The type of the entity specific access rule projection.</typeparam>
    public interface IEntityAccessInfo<TEntityAccessRuleSummary> where TEntityAccessRuleSummary : IEntityAccessRuleSummary
    {
        /// <summary>
        /// <para>
        /// Access rules are used to restrict access to a website resource to users
        /// fulfilling certain criteria such as a specific user area or role.
        /// </para>
        /// <para>
        /// Note that access rules do not apply to users from the Cofoundry Admin user
        /// area. They aren't intended to be used to restrict editor access in the admin UI 
        /// but instead are used to restrict public access to website pages and routes.
        /// </para>
        /// </summary>
        public ICollection<TEntityAccessRuleSummary> AccessRules { get; set; }

        /// <summary>
        /// An action to take when a user does not meet the rule criteria.
        /// </summary>
        public AccessRuleViolationAction ViolationAction { get; set; }

        /// <summary>
        /// The user area to restrict access to.
        /// </summary>
        public string UserAreaCodeForLoginRedirect { get; set; }
    }
}
