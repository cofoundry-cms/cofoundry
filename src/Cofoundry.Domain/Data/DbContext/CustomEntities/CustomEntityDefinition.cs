using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// <para>
    /// Custom entity definitions are used to define the identity and
    /// behavior of a custom entity type. This includes meta data such
    /// as the name and description, but also the configuration of
    /// features such as whether the identity can contain a locale
    /// and whether versioning (i.e. auto-publish) is enabled.
    /// </para>
    /// <para>
    /// Definitions are defined in code by implementing ICustomEntityDefinition
    /// but they are also stored in the database to help with queries and data 
    /// integrity.
    /// </para>
    /// <para>
    /// The code definition is the source of truth and the database is updated
    /// at runtime when an entity is added/updated. This is done via 
    /// EnsureCustomEntityDefinitionExistsCommand.
    /// </para>
    /// </summary>
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

        /// <summary>
        /// Indicates whether entities have some kind of ordering.
        /// This is determines by the definition class inheriting
        /// from IOrderableCustomEntityDefinition and indicating 
        /// an ordering other than CustomEntityOrdering.None.
        /// </summary>
        public bool IsOrderable { get; set; }

        /// <summary>
        /// Indicates whether the entities are partitioned by locale
        /// </summary>
        public bool HasLocale { get; set; }

        public virtual EntityDefinition EntityDefinition { get; set; }
    }
}
