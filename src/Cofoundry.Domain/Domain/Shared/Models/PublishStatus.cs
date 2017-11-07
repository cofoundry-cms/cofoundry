using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the two publish states published/unpublished.
    /// </summary>
    public enum PublishStatus
    {
        /// <summary>
        /// Not that this means flagged as published, but the publish
        /// date property must still be checked to determin if the entity
        /// is published.
        /// </summary>
        Published,

        /// <summary>
        /// The entity is currently unpublished, irrespective of te publish
        /// date.
        /// </summary>
        Unpublished
    }
}
