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

        public int CustomEntityVersionId { get; set; }

        public int CustomEntityId { get; set; }

        public int WorkFlowStatusId { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the custom entity.
        /// </summary>
        public string Title { get; set; }

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
