using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// <para>
    /// Custom entities are a flexible system for developer defined
    /// data structures which can be fully managed in the admin panel 
    /// with minimal configuration. The identity for these entities are 
    /// persisted in this CustomEntity table, while the majority of data 
    /// is versioned in the CustomEntityVersion table.
    /// </para>
    /// <para>
    /// The CustomEntityVersion table also contains the custom data model
    /// data, which is serialized as unstructured data. Entity relations
    /// on the unstructured data model are tracked via the 
    /// UnstructuredDataDependency table.
    /// </para>
    /// </summary>
    /// <inheritdoc/>
    public class CustomEntity : ICreateAuditable, IEntityPublishable
    {
        /// <summary>
        /// Database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Unique 6 character code representing the type of custom entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Definition representing the type of custom entity.
        /// </summary>
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        /// <summary>
        /// The string identifier slug which can
        /// be used as a lookup identifier or in the routing 
        /// of the custom entity page. Can be forced to be unique
        /// by a setting on the custom entity definition.
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// Optional locale id assigned to the custom entity
        /// if used in a localized site.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Optional locale assigned to the custom entity
        /// if used in a localized site.
        /// </summary>
        public virtual Locale Locale { get; set; }

        /// <summary>
        /// Optional ordering value applied to the custom entity 
        /// in relation to other custom entities with the same definition.
        /// </summary>
        public int? Ordering { get; set; }

        public string PublishStatusCode { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? LastPublishDate { get; set; }

        /// <summary>
        /// <para>
        /// Custom entities can have one or more version, with a collection
        /// of versions representing the change history of custom entity
        /// data. 
        /// </para>
        /// <para>
        /// Only one draft version can exist at any one time, and 
        /// only one version may be published at any one time. Although
        /// you can revert to a previous version, this is done by copying
        /// the old version data to a new version, so that a full history is
        /// always maintained.
        /// </para>
        /// <param>
        /// Typically you should query for version data via the 
        /// CustomEntityPublishStatusQuery table, which serves as a quicker
        /// look up for an applicable version for various PublishStatusQuery
        /// states.
        /// </param>
        /// </summary>
        public virtual ICollection<CustomEntityVersion> CustomEntityVersions { get; set; } = new List<CustomEntityVersion>();

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 
        /// 'PreferPublished'.
        /// </summary>
        public virtual ICollection<CustomEntityPublishStatusQuery> CustomEntityPublishStatusQueries { get; set; } = new List<CustomEntityPublishStatusQuery>();

        public int CreatorId { get; set; }

        public User Creator { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
