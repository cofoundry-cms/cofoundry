namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an access rule has been updated on a 
    /// page.
    /// </summary>
    public class PageAccessRuleUpdatedMessage : IPageAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the access rule that has been updated.
        /// </summary>
        public int PageAccessRuleId { get; set; }
    }
}
