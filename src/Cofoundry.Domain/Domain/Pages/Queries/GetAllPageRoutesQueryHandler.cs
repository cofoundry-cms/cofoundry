using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetAllPageRoutesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<PageRoute>, IEnumerable<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageRoute>, IEnumerable<PageRoute>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageCache _pageCache;

        public GetAllPageRoutesQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageCache pageCache
            )
        {
            _dbContext = dbContext;
            _queryExecutor = queryExecutor;
            _pageCache = pageCache;
        }

        #endregion
        
        #region execution

        public async Task<IEnumerable<PageRoute>> ExecuteAsync(GetAllQuery<PageRoute> query, IExecutionContext executionContext)
        {
            return await _pageCache.GetOrAddAsync(() =>
            {
                return GetAllPageRoutesAsync(query, executionContext);
            });
        }

        #region private query classes

        private class PageQueryResult
        {
            public PageRoute RoutingInfo { get; set; }
            public int? LocaleId { get; set; }
            public int PageDirectoryId { get; set; }
        }

        private class PageVersionQueryResult
        {
            public int PageId { get; set; }
            public int PageVersionId { get; set; }
            public string Title { get; set; }
            public int WorkFlowStatusId { get; set; }
            public DateTime CreateDate { get; set; }
            public bool ExcludeFromSitemap { get; set; }
            public int PageTemplateId { get; set; }
        }

        private class PageTemplateQueryResult
        {
            public int PageTemplateId { get; set; }
            public bool HasPageRegions { get; set; }
            public bool HasCustomEntityRegions { get; set; }
        }

        #endregion

        #region queries
        
        private IQueryable<PageVersionQueryResult> QueryPageVersions()
        {
            var dbPageVersions = _dbContext
                .PageVersions
                .AsNoTracking()
                .Where(v => !v.IsDeleted)
                .Select(v => new PageVersionQueryResult()
                {
                    PageId = v.PageId,
                    PageVersionId = v.PageVersionId,
                    Title = v.Title,
                    WorkFlowStatusId = v.WorkFlowStatusId,
                    CreateDate = v.CreateDate,
                    ExcludeFromSitemap = v.ExcludeFromSitemap,
                    PageTemplateId = v.PageTemplateId,
                });

            return dbPageVersions;
        }

        private IQueryable<PageQueryResult> QueryPages()
        {
            var dbPages = _dbContext
                .Pages
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new PageQueryResult()
                {
                    RoutingInfo = new PageRoute()
                    {
                        PageId = p.PageId,
                        UrlPath = p.UrlPath,
                        PageType = (PageType)p.PageTypeId,
                        CustomEntityDefinitionCode = p.CustomEntityDefinitionCode,
                        PublishDate = p.PublishDate,
                        PublishStatus = p.PublishStatusCode == PublishStatusCode.Published ? PublishStatus.Published : PublishStatus.Unpublished
                    },
                    LocaleId = p.LocaleId,
                    PageDirectoryId = p.PageDirectoryId
                });

            return dbPages;
        }

        private IQueryable<PageTemplateQueryResult> GetPageTemplates()
        {
            var dbTemplates = _dbContext
                .PageTemplates
                .Select(t => new PageTemplateQueryResult()
                {
                    PageTemplateId = t.PageTemplateId,
                    HasPageRegions = t.PageTemplateRegions.Any(s => !s.IsCustomEntityRegion),
                    HasCustomEntityRegions = t.PageTemplateRegions.Any(s => s.IsCustomEntityRegion)
                });

            return dbTemplates;
        }

        #endregion

        private async Task<PageRoute[]> GetAllPageRoutesAsync(GetAllQuery<PageRoute> query, IExecutionContext executionContext)
        {
            var dbPages = await QueryPages().ToListAsync();
            var dbPageVersions = await QueryPageVersions().ToListAsync();
            var pageDirectories = (await _queryExecutor.GetAllAsync<PageDirectoryRoute>(executionContext)).ToDictionary(d => d.PageDirectoryId);
            var templates = await GetPageTemplates().ToDictionaryAsync(t => t.PageTemplateId);
            var allLocales = await _queryExecutor.GetAllAsync<ActiveLocale>(executionContext);

            var routes = Map(dbPages, dbPageVersions, pageDirectories, templates, allLocales);

            return routes.ToArray();
        }

        private List<PageRoute> Map(
            List<PageQueryResult> dbPages,
            List<PageVersionQueryResult> dbPageVersions,
            Dictionary<int, PageDirectoryRoute> pageDirectories,
            Dictionary<int, PageTemplateQueryResult> templates,
            IEnumerable<ActiveLocale> activeLocales
            )
        {
            var routes = new List<PageRoute>();

            foreach (var dbPage in dbPages)
            {
                var pageRoute = dbPage.RoutingInfo;

                // Page directory will be null if it is inactive or has an inactive parent.
                pageRoute.PageDirectory = pageDirectories.GetOrDefault(dbPage.PageDirectoryId);
                if (pageRoute.PageDirectory == null) continue;

                // Configure Version Info
                SetPageVersions(pageRoute, dbPageVersions, templates);
                if (!pageRoute.Versions.Any()) continue;

                // Configure Locale
                string directoryPath = null;
                if (dbPage.LocaleId.HasValue)
                {
                    pageRoute.Locale = activeLocales.FirstOrDefault(l => l.LocaleId == dbPage.LocaleId.Value);
                    EntityNotFoundException.ThrowIfNull(pageRoute.Locale, dbPage.LocaleId);

                    directoryPath = pageRoute
                        .PageDirectory
                        .LocaleVariations
                        .Where(v => v.LocaleId == pageRoute.Locale.LocaleId)
                        .Select(v => v.FullUrlPath)
                        .FirstOrDefault();
                }

                if (directoryPath == null)
                {
                    directoryPath = pageRoute.PageDirectory.FullUrlPath;
                }

                // Set Full Path
                pageRoute.FullPath = CreateFullPath(directoryPath, pageRoute.UrlPath, pageRoute.Locale);
                routes.Add(pageRoute);
            }
            return routes;
        }

        private void SetPageVersions(PageRoute routingInfo, IEnumerable<PageVersionQueryResult> dbPageVersions, Dictionary<int, PageTemplateQueryResult> templates)
        {
            var versions = dbPageVersions
                .Where(v => v.PageId == routingInfo.PageId)
                .OrderByDescending(v => (WorkFlowStatus)v.WorkFlowStatusId == WorkFlowStatus.Published)
                .ThenByDescending(v => (WorkFlowStatus)v.WorkFlowStatusId == WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate)
                .ToList();

            routingInfo.Versions = versions
                .Select(v => MapVersion(routingInfo, v, templates))
                .ToArray();

            var latestVersion = versions.FirstOrDefault();

            // There should always be a latest version - but technically it is possible that one doesn't exist.
            if (latestVersion != null)
            {
                routingInfo.Title = latestVersion.Title;
                routingInfo.ShowInSiteMap = !latestVersion.ExcludeFromSitemap;
            }
            else
            {
                // Invalid route, remove the versions collection
                routingInfo.Versions = Array.Empty<PageVersionRoute>();
            }

            routingInfo.HasDraftVersion = routingInfo.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Draft);
            routingInfo.HasPublishedVersion = routingInfo.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);
        }

        private PageVersionRoute MapVersion(PageRoute routingInfo, PageVersionQueryResult version, Dictionary<int, PageTemplateQueryResult> templates)
        {
            var versionRouting = new PageVersionRoute();
            versionRouting.WorkFlowStatus = (WorkFlowStatus)version.WorkFlowStatusId;
            versionRouting.Title = version.Title;
            versionRouting.CreateDate = version.CreateDate;
            versionRouting.VersionId = version.PageVersionId;

            var template = templates.GetOrDefault(version.PageTemplateId);
            if (template != null)
            {
                versionRouting.PageTemplateId = version.PageTemplateId;
                versionRouting.HasCustomEntityRegions = template.HasCustomEntityRegions;
                versionRouting.HasPageRegions = template.HasPageRegions;
            }

            return versionRouting;
        }

        private string CreateFullPath(string path1, string path2, ActiveLocale locale)
        {
            string fullPath = null;

            if (string.IsNullOrWhiteSpace(path2))
            {
                fullPath = path1;
            }
            else if (path1.EndsWith("/"))
            {
                fullPath = path1 + path2;
            }
            else
            {
                fullPath = path1 + "/" + path2;
            }

            if (locale == null) return fullPath;

            var localePath = "/" + locale.IETFLanguageTag.ToLowerInvariant();
            if (fullPath == "/") return localePath;

            fullPath = localePath + fullPath;

            return fullPath;
        }

        #endregion
        
        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageRoute> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
