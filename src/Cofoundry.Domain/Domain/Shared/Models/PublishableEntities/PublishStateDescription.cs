using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A descrition of publishing state at a point in time, which may depend on
    /// a scheduled publish date, e.g. Published, Unpublished or PendingPublish.
    /// </summary>
    public enum PublishStateDescription
    {
        /// <summary>
        /// The entity is currently unpublished, irrespective of the publish
        /// date.
        /// </summary>
        Unpublished,

        /// <summary>
        /// The entity is published and the scheduled publish date
        /// has past.
        /// </summary>
        Published,

        /// <summary>
        /// The entity is published, but the publish date is in the future
        /// and so should not be shown as published yet.
        /// </summary>
        PendingPublish
    }
}
