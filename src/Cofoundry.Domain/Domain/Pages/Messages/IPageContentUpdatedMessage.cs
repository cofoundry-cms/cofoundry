namespace Cofoundry.Domain
{
    /// <summary>
    /// Indicates that the content of a page has changed e.g. any content elements like 
    /// blocks or other page properties. This is also triggered when a new page has been 
    /// added.
    /// </summary>
    public interface IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page entity that the content change affects.
        /// </summary>
        int PageId { get; }

        /// <summary>
        /// Indicates whether the content change affected the published version.
        /// </summary>
        bool HasPublishedVersionChanged { get; }
    }
}
