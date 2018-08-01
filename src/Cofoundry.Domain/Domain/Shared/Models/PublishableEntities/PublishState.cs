using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the publish state of a versioned entity
    /// at a point in time. This is different to PublishStatus
    /// in that this takes into account the publish date and 
    /// calculates when the entity is pending publish.
    /// </summary>
    public struct PublishState
    {
        /// <summary>
        /// Represents the publish state of a versioned entity
        /// at a point in time. This is different to PublishStatus
        /// in that this takes into account the publish date and 
        /// calculates when the entity is pending publish.
        /// </summary>
        /// <param name="publishStatus">The current PublishStatus of the entity.</param>
        /// <param name="publishDate">The date the entity should be published.</param>
        public PublishState(PublishStatus publishStatus, DateTime? publishDate) 
            : this(DateTime.UtcNow, publishStatus, publishDate)
        {
        }

        /// <summary>
        /// Represents the publish state of a versioned entity
        /// at a point in time. This is different to PublishStatus
        /// in that this takes into account the publish date and 
        /// calculates when the entity is pending publish.
        /// </summary>
        /// <param name="dateNow">
        /// The reference date to use when calculating whether theentity is
        /// published or not.
        /// </param>
        /// <param name="publishStatus">The current PublishStatus of the entity.</param>
        /// <param name="publishDate">The date the entity should be published.</param>
        public PublishState(
            DateTime dateNow,
            PublishStatus publishStatus,
            DateTime? publishDate
            )
        {
            switch (publishStatus)
            {
                case PublishStatus.Published:
                    if (!publishDate.HasValue)
                    {
                        throw new ArgumentException($"{nameof(publishDate)} cannot be null if an entity is published.", nameof(publishDate));
                    }
                    Description = dateNow < publishDate.Value ? PublishStateDescription.PendingPublish : PublishStateDescription.Published;
                    break;
                case PublishStatus.Unpublished:
                    Description = PublishStateDescription.Unpublished;
                    break;
                default:
                    throw new ArgumentException($"Unknown {nameof(PublishStatus)} '{publishStatus}'");
            }

            PublishDate = publishDate;
        }

        /// <summary>
        /// The descrition of the state, e.g. Published, Unpublished
        /// or PendingPublish.
        /// </summary>
        public PublishStateDescription Description { get; private set; }

        /// <summary>
        /// The date at which the entity should be published. This
        /// can be used to schedule an entity to be published at
        /// a later date.
        /// </summary>
        public DateTime? PublishDate { get; private set; }
    }
}
