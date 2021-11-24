namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a new page directory is added.
    /// </summary>
    public class PageDirectoryAddedMessage
    {
        /// <summary>
        /// Id of the page that was added.
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
