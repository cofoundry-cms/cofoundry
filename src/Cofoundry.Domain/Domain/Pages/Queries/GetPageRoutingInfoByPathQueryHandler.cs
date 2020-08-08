using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Attempts to find a matching page route using the supplied path. The path
    /// has to be an absolute match, i.e. the query does not try and find a fall-back similar route.
    /// </summary>
    public class GetPageRoutingInfoByPathQueryHandler 
        : IQueryHandler<GetPageRoutingInfoByPathQuery, PageRoutingInfo>
        , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByPathQuery, PageRoutingInfo>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly IPagePathHelper _pathHelper;

        public GetPageRoutingInfoByPathQueryHandler(
            IQueryExecutor queryExecutor,
            IPagePathHelper pathHelper
            )
        {
            _queryExecutor = queryExecutor;
            _pathHelper = pathHelper;
        }

        #endregion

        #region execution

        public async Task<PageRoutingInfo> ExecuteAsync(GetPageRoutingInfoByPathQuery query, IExecutionContext executionContext)
        {
            // Deal with malformed query
            if (!string.IsNullOrWhiteSpace(query.Path) && !Uri.IsWellFormedUriString(query.Path, UriKind.Relative)) return null;

            var path = _pathHelper.StandardizePath(query.Path);
            var allRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);

            var pageRoutes = allRoutes
                .Where(r => r.FullPath.Equals(path) || (r.PageType == PageType.CustomEntityDetails && IsCustomRoutingMatch(path, r.FullPath)))
                .Where(r => query.IncludeUnpublished || r.IsPublished())
                .Where(r => r.Locale == null || MatchesLocale(r.Locale, query.LocaleId))
                .OrderByDescending(r => r.FullPath.Equals(path))
                .ThenByDescending(r => MatchesLocale(r.Locale, query.LocaleId))
                .ToList();

            PageRoutingInfo result = null;

            if (!pageRoutes.Any()) return result;

            // Exact match
            if (pageRoutes[0].PageType != PageType.CustomEntityDetails)
            {
                result = ToRoutingInfo(pageRoutes[0]);
            }
            else
            {
                var allRules = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityRoutingRulesQuery(), executionContext);

                // I'm only anticipating a single rule to match at the moment, but eventually there might be multiple rules to match e.g. categories page
                foreach (var pageRoute in pageRoutes)
                {
                    // Find a routing rule, matching higher priorities first
                    var rule = allRules
                        .Where(r => r.MatchesRule(query.Path, pageRoute))
                        .OrderBy(r => r.Priority)
                        .ThenBy(r => r.RouteFormat.Length)
                        .FirstOrDefault();

                    if (rule != null)
                    {
                        var customEntityRouteQuery = rule.ExtractRoutingQuery(query.Path, pageRoute);
                        var customEntityRoute = await _queryExecutor.ExecuteAsync(customEntityRouteQuery, executionContext);
                        if (customEntityRoute != null && (query.IncludeUnpublished  || customEntityRoute.IsPublished()))
                        {
                            return ToRoutingInfo(pageRoute, customEntityRoute, rule);
                        }
                    }
                }
            }

            return result;
        }

        private bool MatchesLocale(ActiveLocale locale, int? localeId)
        {
            var localeIdToCheck = locale == null ? (int?)null : locale.LocaleId;
            return localeId == localeIdToCheck; 
        }

        private bool IsCustomRoutingMatch(string urltoTest, string routeUrl)
        {
            var routePattern = "^" + Regex.Replace(routeUrl, @"{[^\/{}]*}", @"[^\/{}]*").Replace("/", @"\/") + "$";
            var isMatch = Regex.IsMatch(urltoTest, routePattern);
            
            return isMatch;
        }

        private PageRoutingInfo ToRoutingInfo(PageRoute pageRoute, CustomEntityRoute customEntityRoute = null, ICustomEntityRoutingRule rule = null)
        {
            return new PageRoutingInfo()
            {
                PageRoute = pageRoute,
                CustomEntityRoute = customEntityRoute,
                CustomEntityRouteRule = rule
            };
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByPathQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }

}
