using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Custom entity view model data used in a ICustomEntityPageViewModel implementation. This is 
    /// similar to CustomEntityRenderDetails from the domain, but adds a typed display model that 
    /// is mapped from the raw custom entity data model.
    /// </summary>
    /// <typeparam name="TDisplayModel">The type of view model used to represent the custom entity data model when formatted for display.</typeparam>
    public class CustomEntityRenderDetailsViewModel<TDisplayModel> : ICustomEntityRoutable
    {
        public int CustomEntityId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public ActiveLocale Locale { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public TDisplayModel Model { get; set; }

        public ICollection<CustomEntityPageRegionRenderDetails> Regions { get; set; }

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

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// If this custom entity has page routes asspciated with it
        /// they will be included here. Typically you'd only expect a
        /// single page on a site to be associated with a custom entitiy 
        /// details, but it's technically possible to have many.
        /// </summary>
        public ICollection<string> PageUrls { get; set; }
    }
}
