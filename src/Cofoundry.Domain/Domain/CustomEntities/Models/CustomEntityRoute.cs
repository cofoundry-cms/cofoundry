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
    public class CustomEntityRoute
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
        /// The unique string identifier slug which can
        /// be used in the routing of the custom entity page.
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
        /// Routing information particular to specific versions.
        /// </summary>
        public ICollection<CustomEntityVersionRoute> Versions { get; set; }

        #region public method

        /// <summary>
        /// Determines if the custom entity is published at this moment in time,
        /// checking the published status, the publish date and checking
        /// to make sure there is a published version.
        /// </summary>
        public bool IsPublished()
        {
            var isPublished = PublishStatus == PublishStatus.Published
                && PublishDate <= DateTime.UtcNow
                && Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);

            return isPublished;
        }

        #endregion
    }
}
