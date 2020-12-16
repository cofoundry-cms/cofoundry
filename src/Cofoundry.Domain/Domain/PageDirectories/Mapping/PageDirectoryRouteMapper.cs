using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    public class PageDirectoryRouteMapper : IPageDirectoryRouteMapper
    {
        public ICollection<PageDirectoryRoute> Map(IReadOnlyCollection<PageDirectory> dbPageDirectories)
        {
            const string ROOT_PATH = "/";

            var dbRoot = dbPageDirectories.SingleOrDefault(r => !r.ParentPageDirectoryId.HasValue);
            EntityNotFoundException.ThrowIfNull(dbRoot, "ROOT");

            var root = MapRoute(dbRoot);
            var activePageDirectoryRoutes = SetChildRoutes(root, dbPageDirectories).ToList();

            root.FullUrlPath = ROOT_PATH;

            return activePageDirectoryRoutes;
        }

        private PageDirectoryRoute MapRoute(PageDirectory dbDirectory)
        {
            var route = new PageDirectoryRoute();
            route.Name = dbDirectory.Name;
            route.PageDirectoryId = dbDirectory.PageDirectoryId;
            route.ParentPageDirectoryId = dbDirectory.ParentPageDirectoryId;
            route.UrlPath = dbDirectory.UrlPath;
            route.LocaleVariations = dbDirectory
                .PageDirectoryLocales
                .Select(d => new PageDirectoryRouteLocale()
                {
                    LocaleId = d.LocaleId,
                    UrlPath = d.UrlPath
                })
                .ToList();

            // FullUrlPaths set elsewhere

            return route;
        }

        private IEnumerable<PageDirectoryRoute> SetChildRoutes(PageDirectoryRoute parent, IReadOnlyCollection<PageDirectory> allDbRoutes)
        {
            if (!parent.ParentPageDirectoryId.HasValue)
            {
                yield return parent;
            }

            foreach (var dbRoute in allDbRoutes.Where(r => r.ParentPageDirectoryId == parent.PageDirectoryId))
            {
                var routingInfo = MapRoute(dbRoute);
                routingInfo.FullUrlPath = CombineUrl(parent.FullUrlPath, routingInfo.UrlPath);
                ExpandDirectoryLocales(parent, routingInfo);
                foreach (var childRoute in SetChildRoutes(routingInfo, allDbRoutes))
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
    }
}
