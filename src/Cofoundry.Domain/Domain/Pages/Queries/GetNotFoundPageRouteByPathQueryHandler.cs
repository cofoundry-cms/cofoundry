using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Attempts to find the most relevant 'Not Found' page route by searching
    /// for a 'Not Found' page up the directory tree of a specific path.
    /// </summary>
    public class GetNotFoundPageRouteByPathQueryHandler 
        : IQueryHandler<GetNotFoundPageRouteByPathQuery, PageRoute>
        , IPermissionRestrictedQueryHandler<GetNotFoundPageRouteByPathQuery, PageRoute>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly IPagePathHelper _pathHelper;

        public GetNotFoundPageRouteByPathQueryHandler(
            IQueryExecutor queryExecutor,
            IPagePathHelper pathHelper
            )
        {
            _queryExecutor = queryExecutor;
            _pathHelper = pathHelper;
        }

        #endregion

        public async Task<PageRoute> ExecuteAsync(GetNotFoundPageRouteByPathQuery query, IExecutionContext executionContext)
        {
            if (!string.IsNullOrWhiteSpace(query.Path) && !Uri.IsWellFormedUriString(query.Path, UriKind.Relative)) return null;

            var path = _pathHelper.StandardizePath(query.Path);

            var allRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
            var allNotFoundRoutes = allRoutes
                .Where(r => r.IsPublished() || query.IncludeUnpublished)
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
                notFoundRoute = allNotFoundRoutes
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
