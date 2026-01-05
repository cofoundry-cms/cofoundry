namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="IPublishableEntity"/>.
/// </summary>
public static class IPublishableEntityExtensions
{
    extension(IPublishableEntity entity)
    {
        /// <summary>
        /// Determines if the page is published at this moment in time,
        /// checking the published status, the publish date and checking
        /// to make sure there is a published version.
        /// </summary>
        public bool IsPublished()
        {
            var isPublished = entity.PublishStatus == PublishStatus.Published
                && entity.HasPublishedVersion
                && entity.PublishDate <= DateTime.UtcNow;

            return isPublished;
        }

        /// <summary>
        /// Gets the current publish state at the current moment in time.
        /// </summary>
        public PublishState GetPublishState()
        {
            var publishState = entity.HasPublishedVersion ? entity.PublishStatus : PublishStatus.Unpublished;

            return new PublishState(publishState, entity.PublishDate);
        }
    }
}
