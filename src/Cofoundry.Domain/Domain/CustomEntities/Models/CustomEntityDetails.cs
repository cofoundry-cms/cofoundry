using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityDetails : IPublishableEntity, ICreateAudited
    {
        public int CustomEntityId { get; set; }

        public ActiveLocale Locale { get; set; }

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

        public CustomEntityVersionDetails LatestVersion { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
