using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetNotFoundPageRouteByPathQueryHandler 
        : IAsyncQueryHandler<GetNotFoundPageRouteByPathQuery, PageRoute>
        , IPermissionRestrictedQueryHandler<GetNotFoundPageRouteByPathQuery, PageRoute>
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly PagePathHelper _pathHelper;

        public GetNotFoundPageRouteByPathQueryHandler(
            IQueryExecutor queryExecutor,
            PagePathHelper pathHelper
            )
        {
            _queryExecutor = queryExecutor;
            _pathHelper = pathHelper;
        }

        public async Task<PageRoute> ExecuteAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext)
        {
            if (!string.IsNullOrWhiteSpace(query.Path) && !Uri.IsWellFormedUriString(query.Path, UriKind.Relative)) return null;

            var path = _pathHelper.StandardisePath(query.Path);

            var allRoutes = (await _queryExecutor
                .GetAllAsync<PageRoute>())
                .Where(r => r.IsPublished || query.IncludeUnpublished)
                .Where(r => r.PageType == PageType.NotFound);

            var paths = path
                .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            PageRoute notFoundRoute = null;

            // Work backwards through the path to find a 404 page
            while (notFoundRoute == null)
            {
                var pathToTest = string.Join("/", paths);
                if (string.IsNullOrEmpty(pathToTest))
                {
                    pathToTest = "/";
                }

                // Prefer the specified locale, but fall back to a non-specific locale page
                notFoundRoute = allRoutes
                    .Where(r => r.IsInDirectory(pathToTest) && r.Locale == null || MatchesLocale(r.Locale, query.LocaleId))
                    .OrderByDescending(r => MatchesLocale(r.Locale, query.LocaleId))
                    .FirstOrDefault();

                // After we've checked the root directory, break and return null
                if (paths.Count <= 0)
                {
                    break;
                }

                // Move backwards down the path
                paths.Remove(paths.Last());
            }

            return notFoundRoute;
        }

        private bool MatchesLocale(ActiveLocale locale, int? localeId)
        {
            var localeIdToCheck = locale == null ? (int?)null : locale.LocaleId;
            return localeId == localeIdToCheck;
        }
        
        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetNotFoundPageRouteByPathQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
