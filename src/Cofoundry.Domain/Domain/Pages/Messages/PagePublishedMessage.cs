namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a page draft is published. Note that 
    /// pages can also be published when created, use <see cref="PageAddedMessage"/>
    /// to capture that event.
    /// </summary>
    public class PagePublishedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page that has been published
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// True, obvs
        /// </summary>
        public bool HasPublishedVersionChanged { get { return true; } }
    }
}
