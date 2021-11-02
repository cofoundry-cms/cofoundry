using Cofoundry.Core;
using System;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Combines single-page routing information with custom entity page routing.
    /// A collection of PageRoutingInfo therefore expresses a fuller page routing 
    /// graph with custom entity nodes expanded. Note that custom entity routing
    /// information is only included if the page is PageType.CustomEntityDetails.
    /// </summary>
    public class PageRoutingInfo
    {
        /// <summary>
        /// Routing info for the page part of the expanded route.
        /// </summary>
        public PageRoute PageRoute { get; set; }

        /// <summary>
        /// If the page is PageType.CustomEntityDetails, then this
        /// property will represent the routing information of a single custom 
        /// entity.
        /// </summary>
        public CustomEntityRoute CustomEntityRoute { get; set; }

        /// <summary>
        /// If the page is PageType.CustomEntityDetails, then this
        /// property will the routing rule object that can be used to
        /// construct the full page url when combined with the custom
        /// entity routing data.
        /// </summary>
        public ICustomEntityRoutingRule CustomEntityRouteRule { get; set; }

        /// <summary>
        /// Gets an IVersionRoute that matches the specified publishStatusQuery and version number.
        /// </summary>
        /// <param name="preferCustomEntity">Look for the CustomEntityRouting if its available.</param>
        /// <param name="publishStatusQuery">Specifies how to query for the version e.g. prefer publishes or draft version.</param>
        /// <param name="versionId">Id of a specifc version to look for if using PublishStatusQuery.SpecificVersion.</param>
        public IVersionRoute GetVersionRoute(bool preferCustomEntity, PublishStatusQuery publishStatusQuery, int? versionId = null)
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

        /// <summary>
        /// Gets the current publish state of the route at the current 
        /// moment in time. In the case of custom entity routes, the publish
        /// state from both the page and the custom entity is combined so that
        /// the route can only be published if both entities are published.
        /// </summary>
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
            }

            return publishState;
        }

        /// <summary>
        /// Determines if the <paramref name="user"/> violates any access rules
        /// for this route. If the user cannot access the route then a rule viloation 
        /// is returned. The user may violate several rules in the page and directory 
        /// tree but only the most specific rule set is returned, starting with the page and 
        /// then working back up through the directory tree. 
        /// </summary>
        /// <param name="user">The <see cref="IUserContext"/> to check against access rules.</param>
        /// <returns>
        /// If any rules are violated, then the most specific rule is returned; 
        /// otherwise <see langword="null"/>.
        /// </returns>
        public EntityAccessRuleSet CanAccess(IUserContext user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            EntityInvalidOperationException.ThrowIfNull(this, r => r.PageRoute);

            return PageRoute.CanAccess(user);
        }
    }
}
