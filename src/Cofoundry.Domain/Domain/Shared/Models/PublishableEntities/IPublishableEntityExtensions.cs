using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class IPublishableEntityExtensions
    {
        /// <summary>
        /// Determines if the page is published at this moment in time,
        /// checking the published status, the publish date and checking
        /// to make sure there is a published version.
        /// </summary>
        public static bool IsPublished(this IPublishableEntity entity)
        {
            var isPublished = entity.PublishStatus == PublishStatus.Published
                && entity.HasPublishedVersion
                && entity.PublishDate <= DateTime.UtcNow;

            return isPublished;
        }

        /// <summary>
        /// Gets the current publish state at the current moment in time.
        /// </summary>
        public static PublishState GetPublishState(this IPublishableEntity entity)
        {
            var publishState = entity.HasPublishedVersion ? entity.PublishStatus : PublishStatus.Unpublished;

            return new PublishState(publishState, entity.PublishDate);
        }
    }
}
