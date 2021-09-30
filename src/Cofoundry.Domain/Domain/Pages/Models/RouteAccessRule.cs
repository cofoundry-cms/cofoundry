namespace Cofoundry.Domain
{
    /// <summary>
    /// Access rules are used to control user access to pages for
    /// non-Cofoundry user areas. This object can represent access
    /// rules attached to entities that are involved in page routing
    /// i.e. pages, page directories or custom entities.
    /// </summary>
    public class RouteAccessRule
    {
        /// <summary>
        /// The user area to restrict access to.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Optionally the rule can be restricted to a specific role
        /// within the specified user area.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// An action to take when a user does not meet the rule criteria.
        /// </summary>
        public PageAccessRuleViolationAction ViolationAction { get; set; }
    }
}
