using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A light-weight representation of a custom entity and it's current 
    /// workflow state. Includes model data for the latest version irrespective
    /// of workflow status and should not be used in a version-sensitive context 
    /// such as a public webpage. Used primarily in the admin panel.
    /// </summary>
    public class CustomEntitySummary : IUpdateAudited, IPageRoute//, IPublishableEntity
    {
        /// <summary>
        /// The database id of the custom entity.
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// The six character definition code that represents the type of custom
        /// entity e.g. Blog Post, Project, Product. The definition code is defined
        /// in a class that inherits from ICustomEntityDefinition.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the custom entity.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The string identifier slug which can
        /// be used as a lookup identifier or in the routing 
        /// of the custom entity page. Can be forced to be unique
        /// by a setting on the custom entity definition.
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        /// The full path of the entity including directories and the locale. 
        /// </summary>
        public string FullPath { get; set; }

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
        /// A number representing any custom ordering applied to this custom 
        /// entity. Custom Entities do not have custom ordering by default and 
        /// the IOrderableCustomEntityDefinition interface should be used on your 
        /// definition to imply that custom ordering is available. Doing so will
        /// enable drag-drop ordering in the admin panel.
        /// </summary>
        public int? Ordering { get; set; }

        /// <summary>
        /// Optional locale of the page.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// Custom entity model data deserialized from the database
        /// into the specific data model type related to this custom 
        /// entity. The interface is used as the property type to avoid 
        /// the complications of having a generic version, but you can 
        /// cast the model to the correct data model type in oder to 
        /// access the properties.
        /// </summary>
        public ICustomEntityDataModel Model { get; set; }

        /// <summary>
        /// Simple audit data for custom entity creation.
        /// </summary>
        public UpdateAuditData AuditData { get; set; }
    }
}
