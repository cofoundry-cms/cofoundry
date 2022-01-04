namespace Cofoundry.Domain
{
    /// <summary>
    /// Message published when a custom entity has moved from published to 
    /// draft state.
    /// </summary>
    public class CustomEntityUnPublishedMessage : ICustomEntityContentUpdatedMessage
    {
        /// <summary>
        /// Id of the custom entity that the content change affects
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Definition code of the custom entity that the content change affects
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// True, because the published version has been changed to draft
        /// </summary>
        public bool HasPublishedVersionChanged { get { return true; } }
    }
}
