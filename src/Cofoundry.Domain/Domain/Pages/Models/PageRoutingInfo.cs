using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Encapsulates routing information from a traditional PageRoute as
    /// well as a CustomEntityRoute if it is available.
    /// </summary>
    public class PageRoutingInfo
    {
        public PageRoute PageRoute { get; set; }

        public CustomEntityRoute CustomEntityRoute { get; set; }

        public ICustomEntityRoutingRule CustomEntityRouteRule { get; set; }

        /// <summary>
        /// Gets an IVersionRoute that matches the specified publishStatusQuery and version number.
        /// </summary>
        /// <param name="preferCustomEntity">Look for the CustomEntityRouting if its available.</param>
        /// <param name="publishStatusQuery">Specifies how to query for the version e.g. prefer publishes or draft version.</param>
        /// <param name="versionId">Id of a specifc version to look for if using PublishStatusQuery.SpecificVersion.</param>
        public IVersionRoute GetVersionRoute(bool preferCustomEntity, PublishStatusQuery publishStatusQuery, int? versionId)
        {
            IEnumerable<IVersionRoute> versions = null;

            if (preferCustomEntity && CustomEntityRoute != null)
            {
                versions = CustomEntityRoute.Versions;
            }
            else if (PageRoute != null)
            {
                versions = PageRoute.Versions;
            }

            if (versions != null)
            {
                return versions.GetVersionRouting(publishStatusQuery, versionId);
            }

            return null;
        }

        /// <summary>
        /// Determines if the page is published at this moment in time,
        /// checking the page and custom entity (if available) published status, the publish date 
        /// and checking to make sure there is a published version.
        /// </summary>
        public bool IsPublished()
        {
            if (PageRoute == null)
            {
                throw new InvalidOperationException($"Cannot call {nameof(IsPublished)} if the {nameof(PageRoute)} property is null.");
            }

            var isPublished = PageRoute.IsPublished();

            if (isPublished && CustomEntityRoute != null)
            {
                isPublished = CustomEntityRoute.IsPublished();
            }

            return isPublished;
        }

        public PublishState GetPublishState()
        {
            var publishState = PageRoute.GetPublishState(); 

            if (CustomEntityRoute != null)
            {
                var publishDate = publishState.PublishDate;
                var publishStatus = PageRoute.PublishStatus;

                if (CustomEntityRoute.PublishDate > publishDate)
                {
                    publishDate = CustomEntityRoute.PublishDate;
                }

                if (publishStatus == PublishStatus.Published 
                    && CustomEntityRoute.PublishStatus == PublishStatus.Unpublished)
                {
                    publishStatus = PublishStatus.Unpublished; 
                }

                publishState = new PublishState(publishStatus, publishDate);
                // TODO: YAH: 
                // also, should any other entities conform to IPublishableEntity?
                // That would probably mean a few changes for CustomEntitySummary for example.
            }

            return publishState;
        }
    }
}
