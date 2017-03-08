using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntity : ICreateAuditable
    {
        public CustomEntity()
        {
            CustomEntityVersions = new List<CustomEntityVersion>();
        }

        /// <summary>
        /// Database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public string UrlSlug { get; set; }

        public int? LocaleId { get; set; }

        public int? Ordering { get; set; }

        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        public virtual Locale Locale { get; set; }

        public virtual ICollection<CustomEntityVersion> CustomEntityVersions { get; set; }

        #region ICreateAuditable

        public User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        #endregion
    }
}
