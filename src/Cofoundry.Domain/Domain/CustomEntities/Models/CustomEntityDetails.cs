using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Primarily used in the admin area, the CustomEntityDetails projection 
    /// includes audit data and other additional information that should normally be 
    /// hidden from a customer facing app.
    /// </summary>
    public class CustomEntityDetails : IPublishableEntity, ICreateAudited
    {
        /// <summary>
        /// Database id of the custom entity record.
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
        /// Indicates if the page is marked as published or not, which allows the page
        /// to be shown on the live site if the PublishDate has passed.
        /// </summary>
        public PublishStatus PublishStatus { get; set; }

        /// <summary>
        /// The date after which the page can be shown on the live site.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this entity available.
        /// </summary>
        public bool HasDraftVersion { get; set; }

        /// <summary>
        /// Indicates whether there is a published version of this entity available.
        /// </summary>
        public bool HasPublishedVersion { get; set; }

        /// <summary>
        /// The full path of the default details page. 
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Data for the latest version of the custom entity, which is not
        /// neccessarily published.
        /// </summary>
        public CustomEntityVersionDetails LatestVersion { get; set; }

        /// <summary>
        /// Simple audit data for entity creation.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
