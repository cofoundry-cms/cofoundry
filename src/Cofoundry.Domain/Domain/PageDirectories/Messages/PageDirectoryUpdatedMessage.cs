namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when the properties directly
    /// associated with a page directory have been updated.
    /// </summary>
    public class PageDirectoryUpdatedMessage
    {
        /// <summary>
        /// Id of the page directory that has been updated.
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
