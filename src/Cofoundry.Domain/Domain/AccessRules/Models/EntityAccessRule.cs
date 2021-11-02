namespace Cofoundry.Domain
{
    /// <summary>
    /// Access rules are used to control user access to resources for
    /// non-Cofoundry user areas. This object can represent access
    /// rules attached to entities that are involved in page routing
    /// i.e. pages, page directories or custom entities.
    /// </summary>
    public class EntityAccessRule
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
    }
}
