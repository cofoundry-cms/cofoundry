using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Lookup cache used for quickly finding the correct version for a
    /// specific publish status query e.g. 'Latest', 'Published', 
    /// 'PreferPublished'. These records are generated when custom entities
    /// are published or unpublished.
    /// </summary>
    public partial class CustomEntityPublishStatusQuery
    {
        /// <summary>
        /// Id of the custom entity this record represents. Forms a key
        /// with the PublishStatusQueryId.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Numeric representation of the domain PublishStatusQuery enum.
        /// </summary>
        public short PublishStatusQueryId { get; set; }

        /// <summary>
        /// The id of the version of the  custom entity that should be displayed
        /// for the corresponding PublishStatusQueryId.
        /// </summary>
        public int CustomEntityVersionId { get; set; }

        /// <summary>
        /// Custom entity that this record represents.
        /// </summary>
        public virtual CustomEntity CustomEntity { get; set; }

        /// <summary>
        /// The version of the  custom entity that should be displayed
        /// for the corresponding PublishStatusQueryId.
        /// </summary>
        public virtual CustomEntityVersion CustomEntityVersion { get; set; }

    }
}
