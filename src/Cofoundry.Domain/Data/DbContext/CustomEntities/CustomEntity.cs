using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntity : ICreateAuditable
    {
        public CustomEntity()
        {
            CustomEntityVersions = new List<CustomEntityVersion>();
            CustomEntityPublishStatusQueries = new List<CustomEntityPublishStatusQuery>();
        }

        /// <summary>
        /// Database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Unique 6 letter code representing the type of custom entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

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
        /// Optional ordering value applied to the custom entity 
        /// in relation to other custom entities with the same definition.
        /// </summary>
        public int? Ordering { get; set; }

        /// <summary>
        /// D = draft, P = Published
        /// </summary>
        public string PublishStatusCode { get; set; }

        /// <summary>
        /// Should be set if the status is Published.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Definition representing the type of custom entity.
        /// </summary>
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        /// <summary>
        /// Optional locale assigned to the custom entity
        /// if used in a localized site.
        /// </summary>
        public virtual Locale Locale { get; set; }

        public virtual ICollection<CustomEntityVersion> CustomEntityVersions { get; set; }

        public virtual ICollection<CustomEntityPublishStatusQuery> CustomEntityPublishStatusQueries { get; internal set; }

        #region ICreateAuditable

        public User Creator { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatorId { get; set; }

        #endregion
    }
}
