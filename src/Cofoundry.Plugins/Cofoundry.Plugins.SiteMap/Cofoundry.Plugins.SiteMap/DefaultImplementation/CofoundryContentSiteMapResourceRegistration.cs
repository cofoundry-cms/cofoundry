namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Registration for resources that should appear in the xml site map file.
/// </summary>
public class CofoundryContentSiteMapResourceRegistration : IAsyncSiteMapResourceRegistration
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IUserContextService _userContextService;

    public CofoundryContentSiteMapResourceRegistration(
        IQueryExecutor queryExecutor,
        IPermissionValidationService permissionValidationService,
        IUserContextService userContextService
        )
    {
        _queryExecutor = queryExecutor;
        _permissionValidationService = permissionValidationService;
        _userContextService = userContextService;
    }

    public async Task<IEnumerable<ISiteMapResource>> GetResourcesAsync()
    {
        var userContext = await _userContextService.GetCurrentContextAsync();

        if (!_permissionValidationService.HasPermission<PageReadPermission>(userContext))
        {
            return Array.Empty<ISiteMapResource>();
        }

        var pageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery());
        var allRules = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityRoutingRulesQuery());

        var resources = new List<SiteMapResource>();
        foreach (var pageRoute in pageRoutes.Where(p => p.IsPublished() && p.ShowInSiteMap && p.CanAccess(userContext)))
        {
            if (pageRoute.PageType == PageType.CustomEntityDetails && !string.IsNullOrEmpty(pageRoute.CustomEntityDefinitionCode))
            {
                if (_permissionValidationService.HasCustomEntityPermission<CustomEntityReadPermission>(pageRoute.CustomEntityDefinitionCode, userContext))
                {
                    var routesQuery = new GetCustomEntityRoutesByDefinitionCodeQuery(pageRoute.CustomEntityDefinitionCode);
                    var allCustomEntityRoutes = await _queryExecutor.ExecuteAsync(routesQuery);
                    var pageLocaleId = pageRoute.Locale != null ? pageRoute.Locale.LocaleId : (int?)null;

                    foreach (var customEntityRoute in allCustomEntityRoutes
                        .Where(r => r.Locale == null ? !pageLocaleId.HasValue : r.Locale.LocaleId == pageLocaleId))
                    {
                        var resource = MapCustomEntityResource(pageRoute, customEntityRoute, allRules);
                        if (resource != null)
                        {
                            resources.Add(resource);
                        }
                    }
                }
            }
            else
            {
                resources.Add(MapPageResource(pageRoute));
            }
        }

        return resources;
    }

    private static SiteMapResource? MapCustomEntityResource(
        PageRoute pageRoute,
        CustomEntityRoute customEntityRoute,
        IEnumerable<ICustomEntityRoutingRule> allRules
        )
    {
        var version = customEntityRoute.Versions.GetVersionRouting(PublishStatusQuery.Published);
        if (version == null)
        {
            return null;
        }

        var rule = allRules.FirstOrDefault(r => r.RouteFormat == pageRoute.UrlPath);
        if (rule == null)
        {
            return null;
        }

        var resource = new SiteMapResource
        {
            Url = rule.MakeUrl(pageRoute, customEntityRoute),
            LastModifiedDate = customEntityRoute.LastPublishDate,
            Priority = GetPriority(pageRoute)
        };

        return resource;
    }

    private static SiteMapResource MapPageResource(PageRoute pageRoute)
    {
        var resource = new SiteMapResource
        {
            Url = pageRoute.FullUrlPath,
            LastModifiedDate = pageRoute.LastPublishDate,
            Priority = GetPriority(pageRoute)
        };

        return resource;
    }

    private static decimal GetPriority(PageRoute pageRoute)
    {
        var isDirectoryDefaultPage = pageRoute.IsDirectoryDefaultPage();
        var isInSiteRoot = pageRoute.PageDirectory.IsSiteRoot();

        // Site/Language root
        if (isInSiteRoot && isDirectoryDefaultPage)
        {
            return 1;
        }
        // Directory root
        if (isDirectoryDefaultPage)
        {
            return 0.8m;
        }

        // Directory sub pages
        return 0.6m;
    }
}
