namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when access rules have been updated on a 
    /// page, including any additions, deletions or updates
    /// </summary>
    public class PageAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the page affected by the change.
        /// </summary>
        public int PageId { get; set; }
    }
}
