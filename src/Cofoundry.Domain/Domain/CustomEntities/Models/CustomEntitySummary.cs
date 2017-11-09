using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntitySummary : IUpdateAudited, IPageRoute
    {
        public int CustomEntityId { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        /// <summary>
        /// The full path of the entity including directories and the locale. 
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// True if the page is published and the publish date has passed.
        /// </summary>
        public bool IsPublished { get; set; }

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
        public bool HasDraft { get; set; }

        public int? Ordering { get; set; }

        /// <summary>
        /// Optional locale of the page.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        public ICustomEntityDataModel Model { get; set; }

        public UpdateAuditData AuditData { get; set; }
    }
}
