using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using System.Data.Entity;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all web directories as WebDirectoryRoute objects. The results of this query are cached.
    /// </summary>
    public class GetAllWebDirectoryRoutesQueryHandler 
        : IQueryHandler<GetAllQuery<WebDirectoryRoute>, IEnumerable<WebDirectoryRoute>>
        , IAsyncQueryHandler<GetAllQuery<WebDirectoryRoute>, IEnumerable<WebDirectoryRoute>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<WebDirectoryRoute>, IEnumerable<WebDirectoryRoute>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetAllWebDirectoryRoutesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public IEnumerable<WebDirectoryRoute> Execute(GetAllQuery<WebDirectoryRoute> query, IExecutionContext executionContext)
        {
            var allWebDirectories = Query().ToList();
            var activeWebRoutes = Map(allWebDirectories);

            return activeWebRoutes;
        }

        public async Task<IEnumerable<WebDirectoryRoute>> ExecuteAsync(GetAllQuery<WebDirectoryRoute> query, IExecutionContext executionContext)
        {
            var allWebDirectories = await Query().ToListAsync();
            var activeWebRoutes = Map(allWebDirectories);

            return activeWebRoutes;
        }

        #endregion

        #region helpers
        
        private IQueryable<WebDirectoryRoute> Query()
        {
            return _dbContext
                .WebDirectories
                .AsNoTracking()
                .Where(w => w.IsActive)
                .ProjectTo<WebDirectoryRoute>();
        }

        private List<WebDirectoryRoute> Map(List<WebDirectoryRoute> allWebDirectories)
        {
            var root = allWebDirectories.SingleOrDefault(r => !r.ParentWebDirectoryId.HasValue);
            EntityNotFoundException.ThrowIfNull(root, "ROOT");
            var activeWebRoutes = SetChildRoutes(root, allWebDirectories).ToList();
            root.FullUrlPath = "/";

            return activeWebRoutes;
        }

        private IEnumerable<WebDirectoryRoute> SetChildRoutes(WebDirectoryRoute parent, List<WebDirectoryRoute> allRoutes)
        {
            if (!parent.ParentWebDirectoryId.HasValue)
            {
                yield return parent;
            }

            foreach (var routingInfo in allRoutes.Where(r => r.ParentWebDirectoryId == parent.WebDirectoryId))
            {
                routingInfo.FullUrlPath = CombineUrl(parent.FullUrlPath, routingInfo.UrlPath);
                ExpandDirectoryLocales(parent, routingInfo);
                foreach (var childRoute in SetChildRoutes(routingInfo, allRoutes))
                {
                    yield return childRoute;
                }

                yield return routingInfo;
            }
        }

        /// <summary>
        /// If a locale is defined in a parent directory but not in the child we define one 
        /// using the parent info. This is so we have all locale permitations of this directory even if 
        /// locales are defined further down the directory tree.
        /// </summary>
        private void ExpandDirectoryLocales(WebDirectoryRoute parent, WebDirectoryRoute routingInfo)
        {
            var locales = routingInfo.LocaleVariations.ToList();

            var missingLocales = parent
                .LocaleVariations
                .Where(pl => !locales.Any(l => l.LocaleId == pl.LocaleId))
                .Select(pl => new WebDirectoryRouteLocale()
                {
                    LocaleId = pl.LocaleId,
                    UrlPath = routingInfo.UrlPath
                });

            locales.AddRange(missingLocales);

            foreach (var locale in locales)
            {
                locale.FullUrlPath = CombineUrl(parent.FullUrlPath, locale.UrlPath);
            }

            routingInfo.LocaleVariations = locales;
        }

        private string CombineUrl(string path1, string path2)
        {
            return string.Join("/", path1, path2);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<WebDirectoryRoute> command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
