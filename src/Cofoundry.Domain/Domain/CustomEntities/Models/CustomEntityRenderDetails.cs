using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Data used for rendering a specific version of a custom entity
    /// out to a page.
    /// </summary>
    public class CustomEntityRenderDetails : ICustomEntityRoutable
    {
        public int CustomEntityId { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public int CustomEntityVersionId { get; set; }

        public int? Ordering { get; set; }

        public ActiveLocale Locale { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        /// <summary>
        /// WorkFlowStatus of the version that this instance represents. The version
        /// may not always be the latest version and is dependent on the query that
        /// was used to load this instance, typically using a PublishStatusQuery value.
        /// </summary>
        public WorkFlowStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// Indicates if the page is marked as published or not, which allows the page
        /// to be shown on the live site if the PublishDate has passed.
        /// </summary>
        public PublishStatus PublishStatus { get; set; }

        /// <summary>
        /// The date after which the page can be shown on the live site.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        public ICustomEntityDataModel Model { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<CustomEntityPageRegionRenderDetails> Regions { get; set; }

        /// <summary>
        /// If this custom entity has page routes asspciated with it
        /// they will be included here. Typically you'd only expect a
        /// single page on a site to be associated with a custom entitiy 
        /// details, but it's technically possible to have many.
        /// </summary>
        public ICollection<string> PageUrls { get; set; }
    }
}
