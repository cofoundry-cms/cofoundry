namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that the access rules directly associated with an
    /// individual page has changed e.g. added, updated 
    /// or deleted. This message is not dispatched for any inherited
    /// rule changes from parent directories.
    /// </summary>
    public interface IPageAccessRulesUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has been updated.
        /// </summary>
        int PageId { get; }
    }
}
