namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when an access rule has been added to a 
    /// page directory. This message is not dispatched for child 
    /// directories that inherit the rule.
    /// </summary>
    public class PageDirectoryAccessRuleAddedMessage : IPageDirectoryAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the directory affected by the change
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// The id of the newly added access rule.
        /// </summary>
        public int PageDirectoryAccessRuleId { get; set; }
    }
}
