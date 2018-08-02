using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Contains a small amount of custom entity identity and
    /// page routing information. These route objects are cached 
    /// in order to make routing lookups speedy.
    /// </summary>
    public class CustomEntityRoute : IPublishableEntity
    {
        /// <summary>
        /// Unique 6 letter code representing the type of custom entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Optional locale assigned to the custom entity
        /// if used in a localized site.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// The string identifier slug which can
        /// be used as a lookup identifier or in the routing 
        /// of the custom entity page. Can be forced to be unique
        /// by a setting on the custom entity definition.
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// Indicates if the custom entity is marked as published or not, which allows the 
        /// custom entity to be shown on the live site if the PublishDate has passed.
        /// </summary>
        public PublishStatus PublishStatus { get; set; }

        /// <summary>
        /// The date after which the custom entity can be shown on the live site.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this page available.
        /// </summary>
        public bool HasDraftVersion { get; set; }

        /// <summary>
        /// Indicates whether there is a published version of this page available.
        /// </summary>
        public bool HasPublishedVersion { get; set; }

        /// <summary>
        /// Optional ordering value applied to the custom entity 
        /// in relation to other custom entities with the same definition.
        /// </summary>
        public int? Ordering { get; set; }

        /// <summary>
        /// Routing information particular to specific versions.
        /// </summary>
        public ICollection<CustomEntityVersionRoute> Versions { get; set; }
    }
}
