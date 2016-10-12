using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersion : ICreateAuditable, IEntityVersion
    {
        public CustomEntityVersion()
        {
            CustomEntityVersionPageModules = new List<CustomEntityVersionPageModule>();
        }

        public int CustomEntityVersionId { get; set; }

        public int CustomEntityId { get; set; }

        public int WorkFlowStatusId { get; set; }

        public string Title { get; set; }

        public string SerializedData { get; set; }

        public virtual CustomEntity CustomEntity { get; set; }

        public virtual ICollection<CustomEntityVersionPageModule> CustomEntityVersionPageModules { get; set; }

        #region ICreateAuditable

        public User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        #endregion
    }
}
