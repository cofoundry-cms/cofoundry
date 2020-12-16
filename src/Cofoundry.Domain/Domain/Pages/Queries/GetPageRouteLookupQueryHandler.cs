using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a dictionary lookup of page routing data for all pages. The 
    /// PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetPageRouteLookupQueryHandler
        : IQueryHandler<GetPageRouteLookupQuery, IDictionary<int, PageRoute>>
        , IPermissionRestrictedQueryHandler<GetPageRouteLookupQuery, IDictionary<int, PageRoute>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageCache _pageCache;

        public GetPageRouteLookupQueryHandler(
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
        
        public async Task<IDictionary<int, PageRoute>> ExecuteAsync(GetPageRouteLookupQuery query, IExecutionContext executionContext)
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

        private class PageVersionQueryResult : IEntityVersion
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
                .FilterActive()
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
                .FilterActive()
                .Select(p => new PageQueryResult()
                {
                    RoutingInfo = new PageRoute()
                    {
                        PageId = p.PageId,
                        UrlPath = p.UrlPath,
                        PageType = (PageType)p.PageTypeId,
                        CustomEntityDefinitionCode = p.CustomEntityDefinitionCode,
                        PublishDate = DbDateTimeMapper.AsUtc(p.PublishDate),
                        PublishStatus = PublishStatusMapper.FromCode(p.PublishStatusCode)
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
                .FilterActive()
                .Select(t => new PageTemplateQueryResult()
                {
                    PageTemplateId = t.PageTemplateId,
                    HasPageRegions = t.PageTemplateRegions.Any(s => !s.IsCustomEntityRegion),
                    HasCustomEntityRegions = t.PageTemplateRegions.Any(s => s.IsCustomEntityRegion)
                });

            return dbTemplates;
        }

        #endregion

        private async Task<IDictionary<int, PageRoute>> GetAllPageRoutesAsync(GetPageRouteLookupQuery query, IExecutionContext executionContext)
        {
            var dbPages = await QueryPages().ToListAsync();
            var dbPageVersions = await QueryPageVersions().ToListAsync();
            var pageDirectories = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery(), executionContext);
            var pageDirectoryLookup = pageDirectories.ToDictionary(d => d.PageDirectoryId);
            var templates = await GetPageTemplates().ToDictionaryAsync(t => t.PageTemplateId);
            var allLocales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);

            var routes = Map(dbPages, dbPageVersions, pageDirectoryLookup, templates, allLocales);

            return routes;
        }

        private Dictionary<int, PageRoute> Map(
            List<PageQueryResult> dbPages,
            List<PageVersionQueryResult> dbPageVersions,
            Dictionary<int, PageDirectoryRoute> pageDirectories,
            Dictionary<int, PageTemplateQueryResult> templates,
            ICollection<ActiveLocale> activeLocales
            )
        {
            var routes = new Dictionary<int, PageRoute>();

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
                routes.Add(pageRoute.PageId, pageRoute);
            }
            return routes;
        }

        private void SetPageVersions(
            PageRoute pageRoute,
            ICollection<PageVersionQueryResult> dbPageVersions, 
            Dictionary<int, PageTemplateQueryResult> templates
            )
        {
            bool hasLatestPublishVersion = false;
            pageRoute.Versions = new List<PageVersionRoute>(dbPageVersions.Count);

            var orderedDbVersions = dbPageVersions
                .Where(v => v.PageId == pageRoute.PageId)
                .OrderByLatest();

            foreach (var dbVersion in orderedDbVersions)
            {
                var mappedVersion = MapVersion(pageRoute, dbVersion, templates);
                if (!hasLatestPublishVersion && mappedVersion.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    mappedVersion.IsLatestPublishedVersion = true;
                    hasLatestPublishVersion = true;
                }

                pageRoute.Versions.Add(mappedVersion);
            }

            var latestVersion = orderedDbVersions.FirstOrDefault();

            // There should always be a latest version - but technically it is possible that one doesn't exist.
            if (latestVersion != null)
            {
                pageRoute.Title = latestVersion.Title;
                pageRoute.ShowInSiteMap = !latestVersion.ExcludeFromSitemap;
            }
            else
            {
                // Invalid route, remove the versions collection
                pageRoute.Versions = Array.Empty<PageVersionRoute>();
            }

            pageRoute.HasDraftVersion = pageRoute.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Draft);
            pageRoute.HasPublishedVersion = pageRoute.Versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);
        }

        private PageVersionRoute MapVersion(
            PageRoute routingInfo, 
            PageVersionQueryResult version, 
            Dictionary<int, PageTemplateQueryResult> templates
            )
        {
            var versionRouting = new PageVersionRoute();
            versionRouting.WorkFlowStatus = (WorkFlowStatus)version.WorkFlowStatusId;
            versionRouting.Title = version.Title;
            versionRouting.CreateDate = DbDateTimeMapper.AsUtc(version.CreateDate);
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageRouteLookupQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
