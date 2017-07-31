using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all page directories as PageDirectoryRoute objects. The results of this 
    /// query are cached.
    /// </summary>
    public class GetAllPageDirectoryRoutesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<PageDirectoryRoute>, IEnumerable<PageDirectoryRoute>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageDirectoryRoute>, IEnumerable<PageDirectoryRoute>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetAllPageDirectoryRoutesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution
        
        public async Task<IEnumerable<PageDirectoryRoute>> ExecuteAsync(GetAllQuery<PageDirectoryRoute> query, IExecutionContext executionContext)
        {
            var allPageDirectories = await Query().ToListAsync();
            var activeWebRoutes = Map(allPageDirectories);

            return activeWebRoutes;
        }

        #endregion

        #region helpers
        
        private IQueryable<PageDirectoryRoute> Query()
        {
            return _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(d => d.IsActive)
                .ProjectTo<PageDirectoryRoute>();
        }

        private List<PageDirectoryRoute> Map(List<PageDirectoryRoute> allPageDirectories)
        {
            var root = allPageDirectories.SingleOrDefault(r => !r.ParentPageDirectoryId.HasValue);
            EntityNotFoundException.ThrowIfNull(root, "ROOT");
            var activePageDirectoryRoutes = SetChildRoutes(root, allPageDirectories).ToList();
            root.FullUrlPath = "/";

            return activePageDirectoryRoutes;
        }

        private IEnumerable<PageDirectoryRoute> SetChildRoutes(PageDirectoryRoute parent, List<PageDirectoryRoute> allRoutes)
        {
            if (!parent.ParentPageDirectoryId.HasValue)
            {
                yield return parent;
            }

            foreach (var routingInfo in allRoutes.Where(r => r.ParentPageDirectoryId == parent.PageDirectoryId))
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
        private void ExpandDirectoryLocales(PageDirectoryRoute parent, PageDirectoryRoute routingInfo)
        {
            var locales = routingInfo.LocaleVariations.ToList();

            var missingLocales = parent
                .LocaleVariations
                .Where(pl => !locales.Any(l => l.LocaleId == pl.LocaleId))
                .Select(pl => new PageDirectoryRouteLocale()
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageDirectoryRoute> command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
