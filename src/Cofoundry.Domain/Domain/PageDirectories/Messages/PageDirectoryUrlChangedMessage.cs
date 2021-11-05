namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when properties that affect the url 
    /// of a page directory has changed (e.g. ParentPageDirectoryId, 
    /// UrlSlug etc). This message is not triggered if the url
    /// indirectly changes e.g. if a parent directory changes.
    /// </summary>
    public class PageDirectoryUrlChangedMessage
    {
        /// <summary>
        /// Id of the page directory that has the url updated
        /// </summary>
        public int PageDirectoryId { get; set; }
    }
}
