using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityDefinition
    {
        /// <summary>
        /// Unique 6 letter code representing the entity (use uppercase)
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Indicates whether the UrlSlug property should be treated
        /// as a unique property and be validated as such. This will also affect
        /// the routing templates available for this entity because some routes require
        /// a unique slug.
        /// </summary>
        public bool ForceUrlSlugUniqueness { get; set; }

        public bool IsOrderable { get; set; }

        /// <summary>
        /// Indicates whether the entities are partitioned by locale
        /// </summary>
        public bool HasLocale { get; set; }

        public virtual EntityDefinition EntityDefinition { get; set; }
    }
}
