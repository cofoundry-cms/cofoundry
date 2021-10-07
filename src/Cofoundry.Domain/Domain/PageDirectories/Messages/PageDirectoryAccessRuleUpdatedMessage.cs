namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an access rule has been updated on a 
    /// page directory. This message is not dispatched for child 
    /// directories that inherit the rule.
    /// </summary>
    public class PageDirectoryAccessRuleUpdatedMessage : IPageDirectoryAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the directory affected by the change
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The id of the access rule that has been updated.
        /// </summary>
        public int PageDirectoryAccessRuleId { get; set; }
    }
}
