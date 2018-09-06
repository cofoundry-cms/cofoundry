using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the two publish states published/unpublished. Note that
    /// this is different to the PublishState, which takes into account
    /// the publish date and marks the state at a point in time.
    /// </summary>
    public enum PublishStatus
    {
        /// <summary>
        /// Note that this means flagged as published, but the publish
        /// date property must still be checked to determin if the entity
        /// is published.
        /// </summary>
        Published,

        /// <summary>
        /// The entity is currently unpublished, irrespective of the publish
        /// date.
        /// </summary>
        Unpublished
    }
}
