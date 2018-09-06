using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An entity that used the standard publishing mechanism e.g.
    /// a page or custom entity.
    /// </summary>
    public interface IPublishableEntity
    {
        /// <summary>
        /// Indicates if the entity is marked as published or not, which allows the entity
        /// to be shown on the live site if the PublishDate has passed.
        /// </summary>
        PublishStatus PublishStatus { get; set; }

        /// <summary>
        /// The date after which the entity can be shown on the live site.
        /// </summary>
        DateTime? PublishDate { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this entity available.
        /// </summary>
        bool HasDraftVersion { get; set; }

        /// <summary>
        /// Indicates whether there is a published version of this entity available.
        /// </summary>
        bool HasPublishedVersion { get; set; }
    }
}
