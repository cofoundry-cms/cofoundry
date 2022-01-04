namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when the data directly
    /// associated with a page has been updated. To be notified
    /// when any part of a page (including versions) changes you
    /// should subscribe to <see cref="IPageContentUpdatedMessage"/>.
    /// </summary>
    public class PageUpdatedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has been updated.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// True if the page was in a published state when it was updated.
        /// </summary>
        public bool HasPublishedVersionChanged { get; set; }
    }
}
