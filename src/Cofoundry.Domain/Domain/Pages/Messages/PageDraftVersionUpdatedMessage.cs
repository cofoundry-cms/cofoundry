namespace Cofoundry.Domain
{
    /// <summary>
    /// This message is published when page draft has been 
    /// updated. To be notified when any part of a page changes 
    /// (including page data and publish changes) you should 
    /// subscribe to <see cref="IPageContentUpdatedMessage"/>.
    /// </summary>
    public class PageDraftVersionUpdatedMessage : IPageContentUpdatedMessage
    {
        /// <summary>
        /// Id of the page with the draft version being updated
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Because the draft version has been modified this will
        /// always be false
        /// </summary>
        public bool HasPublishedVersionChanged { get { return false; } }

        /// <summary>
        /// Id of the page version that has been updated
        /// </summary>
        public int PageVersionId { get; set; }
    }
}
