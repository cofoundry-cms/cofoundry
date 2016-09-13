using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Core;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UrlSlugCustomEntityRoutingRule : ICustomEntityRoutingRule
    {
        public string RouteFormat
        {
            get { return "{UrlSlug}"; }
        }

        public int Priority
        {
            get { return (int)RoutingRulePriority.Normal; }
        }

        public bool RequiresUniqueUrlSlug
        {
            get { return true; }
        }

        public bool MatchesRule(string url, PageRoute pageRoute)
        {
            Condition.Requires(url).IsNotNullOrWhiteSpace();
            Condition.Requires(pageRoute).IsNotNull();

            var slugUrlPart = GetRoutingPart(url, pageRoute);
            if (string.IsNullOrEmpty(slugUrlPart)) return false;
            var isMatch = Regex.IsMatch(slugUrlPart, RegexLibary.SlugCaseInsensitive);

            return isMatch;
        }

        public IQuery<CustomEntityRoute> ExtractRoutingQuery(string url, PageRoute pageRoute)
        {
            Condition.Requires(url).IsNotNullOrWhiteSpace();
            Condition.Requires(pageRoute).IsNotNull();
            Condition.Requires(MatchesRule(url, pageRoute)).IsTrue();

            var slugUrlPart = GetRoutingPart(url, pageRoute);

            var query = new GetCustomEntityRouteByPathQuery()
            {
                CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode,
                UrlSlug = slugUrlPart
            };

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

            return pageRoute.FullPath.Replace(RouteFormat, entityRoute.UrlSlug);
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
