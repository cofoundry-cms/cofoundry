namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an access rule has been removed from a page.
    /// </summary>
    public class PageAccessRuleDeletedMessage : IPageAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the access rule that has been deleted.
        /// </summary>
        public int PageAccessRuleId { get; set; }
    }
}
