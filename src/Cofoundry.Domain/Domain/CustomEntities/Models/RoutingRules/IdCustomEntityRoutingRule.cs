using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A routing rule that uses only the CustomEntityId property to match the
    /// custom entity.
    /// </summary>
    public class IdCustomEntityRoutingRule : ICustomEntityRoutingRule
    {
        /// <summary>
        /// A string representation of the route format e.g.  "{Id}/{UrlSlug}". Used as a display value
        /// but also as the unique identifier for the rule, so it shouldn't clash with any other routing rule.
        /// </summary>
        public string RouteFormat
        {
            get { return "{Id}"; }
        }

        /// <summary>
        /// Sets a priority over which rules should be run in case more than one is used in the
        /// same page directory. Custom integer values can be used but use RoutingRulePriority whenever possible
        /// to avoid hardcoding to a specific value.
        /// </summary>
        public int Priority
        {
            get { return (int)RoutingRulePriority.Normal; }
        }

        /// <summary>
        /// Indicates whether this rule can only be used with custom entities with a unique 
        /// url slug, indicated by the ForceUrlSlugUniqueness setting on the 
        /// <see cref="ICustomEntityDefinition"/> implementation.
        /// </summary>
        public bool RequiresUniqueUrlSlug
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates whether the specified url matches this routing rule.
        /// </summary>
        /// <param name="url">The url to test</param>
        /// <param name="pageRoute">The page route already matched to this url.</param>
        public bool MatchesRule(string url, PageRoute pageRoute)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentEmptyException(nameof(url));
            if (pageRoute == null) throw new ArgumentNullException(nameof(pageRoute));

            var routingPart = GetRoutingPart(url, pageRoute);
            if (string.IsNullOrEmpty(routingPart)) return false;

            var isMatch = IntParser.ParseOrDefault(routingPart) > 0;

            return isMatch;
        }

        /// <summary>
        /// Returns a query that can be used to look up the CustomEntityRoute relating 
        /// to the matched entity. Throws an exception if the MatchesRule returns false, so
        /// check this before calling this method.
        /// </summary>
        /// <param name="url">The url to parse custom entity key data from</param>
        /// <param name="pageRoute">The page route matched to the url</param>
        /// <returns>An IQuery object that can used to query for the CustomEntityRoute</returns>
        public IQuery<CustomEntityRoute> ExtractRoutingQuery(string url, PageRoute pageRoute)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentEmptyException(nameof(url));
            if (pageRoute == null) throw new ArgumentNullException(nameof(pageRoute));

            if (!MatchesRule(url, pageRoute))
            {
                throw new ArgumentException(nameof(url) + $" does not match the specified page route. {nameof(ExtractRoutingQuery)} can only be called after a successful page route match.", nameof(url));
            }

            var routingPart = GetRoutingPart(url, pageRoute);
            var customEntityId = IntParser.ParseOrDefault(routingPart);

            var query = new GetCustomEntityRouteByPathQuery();
            query.CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode;
            query.CustomEntityId = customEntityId;

            if (pageRoute.Locale != null)
            {
                query.LocaleId = pageRoute.Locale.LocaleId;
            }

            return query;
        }

        /// <summary>
        /// Transforms the routing specified routing information into a full, relative url.
        /// </summary>
        /// <param name="pageRoute">The matched page route for the url</param>
        /// <param name="entityRoute">The matched custom entity route for the url</param>
        /// <returns>Full, relative url</returns>
        public string MakeUrl(PageRoute pageRoute, CustomEntityRoute entityRoute)
        {
            if (pageRoute == null) throw new ArgumentNullException(nameof(pageRoute));
            if (entityRoute == null) throw new ArgumentNullException(nameof(entityRoute));

            return pageRoute
                .FullUrlPath
                .Replace("{Id}", entityRoute.CustomEntityId.ToString());
        }

        /// <summary>
        /// Extracts the custom entity routing part of the path from 
        /// a <paramref name="url"/> e.g. url "/my-path/123" with Pageroute 
        /// "/my-path/{id}" will return "123".
        /// </summary>
        private string GetRoutingPart(string url, PageRoute pageRoute)
        {
            if (pageRoute.FullUrlPath.IndexOf(RouteFormat) == -1) return null;

            var pathRoot = pageRoute.FullUrlPath.Replace(RouteFormat, string.Empty);
            // if not found or there are other parameters in the route path not resolved.
            if (pathRoot.Contains('{')) return null;

            return url.Substring(pathRoot.Length - 1).Trim('/');
        }
    }
}
