namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when access rules have been updated on a 
    /// page directory, including any additions, deletions or updates. 
    /// This message is not dispatched for child directories that inherit 
    /// the rule.
    /// </summary>
    public class PageDirectoryAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the directory affected by the change
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
