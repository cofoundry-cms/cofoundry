using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IdAndUrlSlugCustomEntityRoutingRule : ICustomEntityRoutingRule
    {
        /// <summary>
        /// id must be an integer, slug is optional
        /// </summary>
        private const string ROUTE_REGEX = @"^(\d+)(?:\/([a-zA-Z0-9-_]+))?$";

        public string RouteFormat
        {
            get { return "{Id}/{UrlSlug}"; }
        }

        public int Priority
        {
            get { return (int)RoutingRulePriority.Normal; }
        }

        public bool RequiresUniqueUrlSlug
        {
            get { return false; }
        }

        public bool MatchesRule(string url, PageRoute pageRoute)
        {
            Condition.Requires(url).IsNotNullOrWhiteSpace();
            Condition.Requires(pageRoute).IsNotNull();

            var routingPart = GetRoutingPart(url, pageRoute);
            if (string.IsNullOrEmpty(routingPart)) return false;

            var isMatch = Regex.IsMatch(routingPart, ROUTE_REGEX);

            return isMatch;
        }

        public IQuery<CustomEntityRoute> ExtractRoutingQuery(string url, PageRoute pageRoute)
        {
            Condition.Requires(url).IsNotNullOrWhiteSpace();
            Condition.Requires(pageRoute).IsNotNull();
            Condition.Requires(MatchesRule(url, pageRoute)).IsTrue();

            var routingPart = GetRoutingPart(url, pageRoute);

            var match = Regex.Match(routingPart, ROUTE_REGEX);

            var query = new GetCustomEntityRouteByPathQuery();
            query.CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode;
            query.CustomEntityId = Convert.ToInt32(match.Groups[1].Value);

            if (pageRoute.Locale != null)
            {
                query.LocaleId = pageRoute.Locale.LocaleId;
            }

            return query;
        }

        public string MakeUrl(PageRoute pageRoute, CustomEntityRoute entityRoute)
        {
            Condition.Requires(pageRoute).IsNotNull();
            Condition.Requires(entityRoute).IsNotNull();

            return pageRoute.FullPath
                .Replace("{Id}", entityRoute.CustomEntityId.ToString())
                .Replace("{UrlSlug}", entityRoute.UrlSlug);
        }

        #region private helpers

        private string GetRoutingPart(string url, PageRoute pageRoute)
        {
            if (pageRoute.FullPath.IndexOf(RouteFormat) == -1) return null;

            var pathRoot = pageRoute.FullPath.Replace(RouteFormat, string.Empty);
            // if not found or there are other parameters in the route path not resolved.
            if (pathRoot.Contains('{')) return null;

            return url.Substring(pathRoot.Length - 1).Trim('/');
        }

        #endregion
    }
}
