using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
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
    /// </summary>
    /// <remarks>
    /// Typically you should query for version data via the 
    /// CustomEntityPublishStatusQuery table, which serves as a quicker
    /// look up for an applicable version for various PublishStatusQuery
    /// states.
    /// </remarks>
    public class CustomEntityVersion : ICreateAuditable, IEntityVersion
    {
        public CustomEntityVersion()
        {
            CustomEntityVersionPageBlocks = new List<CustomEntityVersionPageBlock>();
            CustomEntityPublishStatusQueries = new List<CustomEntityPublishStatusQuery>();
        }

        /// <summary>
        /// Auto-incrementing primary key of the custom entity version
        /// record in the database.
        /// </summary>
        public int CustomEntityVersionId { get; set; }

        /// <summary>
        /// The id of the custom entity this version is parented to.
        /// Custom entities can have many versions, but only one is
        /// published at any one time.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Mapped from the domain enum WorkFlowStatus, this is the workflow 
        /// state of this version e.g. draft/published.
        /// </summary>
        public int WorkFlowStatusId { get; set; }

        /// <summary>
        /// A display-friendly version number that indicates
        /// it's position in the hisotry of all verions of a specific
        /// custom entity. E.g. the first version for a custom entity 
        /// is version 1 and  the 2nd is version 2. The display version 
        /// is unique per custom entity.
        /// </summary>
        public int DisplayVersion { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the custom entity.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The custom entity data model serialized into string data by
        /// IDbUnstructuredDataSerializer, which used JSON serlialization
        /// by default.
        /// </summary>
        public string SerializedData { get; set; }

        /// <summary>
        /// The custom entity this version is parented to.
        /// Custom entities can have many versions, but only one is
        /// published at any one time.
        /// </summary>
        public virtual CustomEntity CustomEntity { get; set; }

        /// <summary>
        /// The dynamic 'Pages' feature can be used to create one or more 
        /// 'custom entity details' page which displays custom enitity data
        /// based on the routing parameters (i.e. the details page in a 
        /// master-details arrangement). This property holds the block data
        /// for the page template regions on any of these pages.
        /// </summary>
        public virtual ICollection<CustomEntityVersionPageBlock> CustomEntityVersionPageBlocks { get; set; }

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 
        /// 'PreferPublished'.
        /// </summary>
        public virtual ICollection<CustomEntityPublishStatusQuery> CustomEntityPublishStatusQueries { get; internal set; }

        #region ICreateAuditable

        /// <summary>
        /// The user that created the custom entity version.
        /// </summary>
        public User Creator { get; set; }

        /// <summary>
        /// The date the custom entity version was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The database id of the user that created the custom entity version.
        /// </summary>
        public int CreatorId { get; set; }

        #endregion
    }
}
