using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
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

        public int CustomEntityId { get; set; }

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

        public virtual CustomEntity CustomEntity { get; set; }

        public virtual ICollection<CustomEntityVersionPageBlock> CustomEntityVersionPageBlocks { get; set; }

        public virtual ICollection<CustomEntityPublishStatusQuery> CustomEntityPublishStatusQueries { get; internal set; }

        #region ICreateAuditable

        public User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        #endregion
    }
}
