using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A page summary contains information used when listing pages in the
    /// admin panel. The model isn't version specific and should not be used 
    /// to render content out to a live page, since the page may not be 
    /// published.
    /// </summary>
    public class PageSummary : ICreateAudited, IPageRoute, IPublishableEntity
    {
        /// <summary>
        /// Database id of the page record.
        /// </summary>
        public int PageId { get; set; }
        
        /// <summary>
        /// The path of the page within the directory.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The full path of the page including directories and the locale. 
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
        /// Indicates whether there is a draft version of this page available.
        /// </summary>
        public bool HasDraftVersion { get; set; }

        /// <summary>
        /// Indicates whether there is a published version of this page available.
        /// </summary>
        public bool HasPublishedVersion { get; set; }

        /// <summary>
        /// The title of the page for the currently published version, falling
        /// back to the draft version is there is no published version.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional locale of the page.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// The page could be a generic page or could have some sort of
        /// special function e.g. NotFound, CustomEntityDetails
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// If this instance is PageType.CustomEntityDetails, this will contain
        /// name of the custom entity definition.
        /// </summary>
        public string CustomEntityName { get; set; }

        /// <summary>
        /// Collection of tag names that this entity is tagged with.
        /// </summary>
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// Simple audit data for entity creation.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
