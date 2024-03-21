﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns a dictionary lookup of page routing data for all pages. The 
/// PageRoute projection is a small page object focused on providing 
/// routing data only. Data returned from this query is cached by 
/// default as it's core to routing and often incorporated in more detailed
/// page projections.
/// </summary>
public class GetPageRouteLookupQueryHandler
    : IQueryHandler<GetPageRouteLookupQuery, IReadOnlyDictionary<int, PageRoute>>
    , IPermissionRestrictedQueryHandler<GetPageRouteLookupQuery, IReadOnlyDictionary<int, PageRoute>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageCache _pageCache;
    private readonly IEntityAccessRuleSetMapper _routeAccessRuleMapper;

    public GetPageRouteLookupQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPageCache pageCache,
        IEntityAccessRuleSetMapper routeAccessRuleMapper
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
        _pageCache = pageCache;
        _routeAccessRuleMapper = routeAccessRuleMapper;
    }

    public async Task<IReadOnlyDictionary<int, PageRoute>> ExecuteAsync(GetPageRouteLookupQuery query, IExecutionContext executionContext)
    {
        return await _pageCache.GetOrAddAsync(() =>
        {
            return GetAllPageRoutesAsync(query, executionContext);
        });
    }

    private async Task<IReadOnlyDictionary<int, PageRoute>> GetAllPageRoutesAsync(GetPageRouteLookupQuery query, IExecutionContext executionContext)
    {
        var dbPages = await GetPagesAsync();
        var dbPageVersionLookup = await GetPageVersionsAsync();
        var pageDirectories = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery(), executionContext);
        var pageDirectoryLookup = pageDirectories.ToDictionary(d => d.PageDirectoryId);
        var templates = await GetPageTemplatesAsync();
        var dbPageAccessRulesLookup = await GetAccessRulesAsync();
        var allLocales = await _queryExecutor.ExecuteAsync(new GetAllActiveLocalesQuery(), executionContext);

        var routes = Map(
            dbPages,
            dbPageVersionLookup,
            pageDirectoryLookup,
            templates,
            dbPageAccessRulesLookup,
            allLocales
            );

        return routes;
    }

    private async Task<IReadOnlyCollection<Page>> GetPagesAsync()
    {
        return await _dbContext
            .Pages
            .AsNoTracking()
            .FilterActive()
            .Include(p => p.AccessRules)
            .ToArrayAsync();
    }

    private async Task<Dictionary<int, IOrderedEnumerable<PageVersionQueryResult>>> GetPageVersionsAsync()
    {
        var result = await _dbContext
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
            })
            .ToArrayAsync();

        return result
            .GroupBy(k => k.PageId)
            .ToDictionary(k => k.Key, v => v.OrderByLatest());
    }

    private Task<Dictionary<int, PageTemplateQueryResult>> GetPageTemplatesAsync()
    {
        return _dbContext
            .PageTemplates
            .AsNoTracking()
            .FilterActive()
            .Select(t => new PageTemplateQueryResult()
            {
                PageTemplateId = t.PageTemplateId,
                HasPageRegions = t.PageTemplateRegions.Any(s => !s.IsCustomEntityRegion),
                HasCustomEntityRegions = t.PageTemplateRegions.Any(s => s.IsCustomEntityRegion)
            })
            .ToDictionaryAsync(t => t.PageTemplateId);
    }

    private async Task<Dictionary<int, IEnumerable<PageAccessRule>>> GetAccessRulesAsync()
    {
        var result = await _dbContext
            .PageAccessRules
            .AsNoTracking()
            .OrderByDefault()
            .ToArrayAsync();

        return result
            .GroupBy(r => r.PageId)
            .ToDictionary(k => k.Key, v => v.AsEnumerable());
    }

    private Dictionary<int, PageRoute> Map(
        IReadOnlyCollection<Page> dbPages,
        Dictionary<int, IOrderedEnumerable<PageVersionQueryResult>> dbPageVersionLookup,
        Dictionary<int, PageDirectoryRoute> pageDirectories,
        Dictionary<int, PageTemplateQueryResult> templates,
        Dictionary<int, IEnumerable<PageAccessRule>> accessRuleLookup,
        IReadOnlyCollection<ActiveLocale> activeLocales
        )
    {
        var routes = new Dictionary<int, PageRoute>();

        foreach (var dbPage in dbPages)
        {
            var pageRoute = new PageRoute()
            {
                PageId = dbPage.PageId,
                UrlPath = dbPage.UrlPath,
                PageType = (PageType)dbPage.PageTypeId,
                CustomEntityDefinitionCode = dbPage.CustomEntityDefinitionCode,
                PublishDate = dbPage.PublishDate,
                LastPublishDate = dbPage.LastPublishDate,
                PublishStatus = PublishStatusMapper.FromCode(dbPage.PublishStatusCode)
            };

            var directory = pageDirectories.GetOrDefault(dbPage.PageDirectoryId);
            if (directory == null)
            {
                // Page directory will be null if it is inactive or has an inactive parent.
                continue;
            }
            pageRoute.PageDirectory = directory;

            // Configure Version Info
            SetPageVersions(pageRoute, dbPageVersionLookup, templates);
            if (pageRoute.Versions.Count == 0)
            {
                continue;
            }

            var accessRules = accessRuleLookup.GetOrDefault(pageRoute.PageId);
            pageRoute.AccessRuleSet = _routeAccessRuleMapper.Map(dbPage);

            // Configure Locale
            string? directoryPath = null;
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
            pageRoute.FullUrlPath = CreateFullPath(directoryPath, pageRoute.UrlPath, pageRoute.Locale);
            routes.Add(pageRoute.PageId, pageRoute);
        }
        return routes;
    }

    private static void SetPageVersions(
        PageRoute pageRoute,
        Dictionary<int, IOrderedEnumerable<PageVersionQueryResult>> dbPageVersionLookup,
        Dictionary<int, PageTemplateQueryResult> templates
        )
    {
        var hasLatestPublishVersion = false;

        var orderedDbVersions = dbPageVersionLookup.GetOrDefault(pageRoute.PageId);

        var versions = new List<PageVersionRoute>();
        foreach (var dbVersion in EnumerableHelper.Enumerate(orderedDbVersions))
        {
            var mappedVersion = MapVersion(dbVersion, templates);
            if (!hasLatestPublishVersion && mappedVersion.WorkFlowStatus == WorkFlowStatus.Published)
            {
                mappedVersion.IsLatestPublishedVersion = true;
                hasLatestPublishVersion = true;
            }

            versions.Add(mappedVersion);
        }
        pageRoute.Versions = versions;

        var latestVersion = orderedDbVersions?.FirstOrDefault();

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

    private static PageVersionRoute MapVersion(
        PageVersionQueryResult version,
        Dictionary<int, PageTemplateQueryResult> templates
        )
    {
        var versionRouting = new PageVersionRoute
        {
            WorkFlowStatus = (WorkFlowStatus)version.WorkFlowStatusId,
            Title = version.Title,
            CreateDate = version.CreateDate,
            VersionId = version.PageVersionId
        };

        var template = templates.GetOrDefault(version.PageTemplateId);
        if (template != null)
        {
            versionRouting.PageTemplateId = version.PageTemplateId;
            versionRouting.HasCustomEntityRegions = template.HasCustomEntityRegions;
            versionRouting.HasPageRegions = template.HasPageRegions;
        }

        return versionRouting;
    }

    private static string CreateFullPath(string path1, string path2, ActiveLocale? locale)
    {
        string fullPath;

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

        if (locale == null)
        {
            return fullPath;
        }

        var localePath = "/" + locale.IETFLanguageTag.ToLowerInvariant();
        if (fullPath == "/")
        {
            return localePath;
        }

        fullPath = localePath + fullPath;

        return fullPath;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRouteLookupQuery query)
    {
        yield return new PageReadPermission();
    }

    private class PageQueryResult
    {
        public required PageRoute RoutingInfo { get; set; }
        public required int? LocaleId { get; set; }
        public required int PageDirectoryId { get; set; }
    }

    private class PageVersionQueryResult : IEntityVersion
    {
        public required int PageId { get; set; }
        public required int PageVersionId { get; set; }
        public required string Title { get; set; }
        public required int WorkFlowStatusId { get; set; }
        public required DateTime CreateDate { get; set; }
        public required bool ExcludeFromSitemap { get; set; }
        public required int PageTemplateId { get; set; }
    }

    private class PageTemplateQueryResult
    {
        public required int PageTemplateId { get; set; }
        public required bool HasPageRegions { get; set; }
        public required bool HasCustomEntityRegions { get; set; }
    }
}
