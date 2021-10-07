namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that the access rules directly associated with an
    /// individual directory has changed e.g. added, updated or deleted. 
    /// This message is not dispatched for child directories that inherit 
    /// the rule.
    /// </summary>
    public interface IPageDirectoryAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the directory that has been updated.
        /// </summary>
        int PageDirectoryId { get; }
    }
}
