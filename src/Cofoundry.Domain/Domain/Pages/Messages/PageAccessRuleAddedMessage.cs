namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an access rule has been added to a page.
    /// </summary>
    public class PageAccessRuleAddedMessage : IPageAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the newly added access rule.
        /// </summary>
        public int PageAccessRuleId { get; set; }
    }
}
