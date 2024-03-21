using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageDirectoryRouteMapper"/>.
/// </summary>
public class PageDirectoryRouteMapper : IPageDirectoryRouteMapper
{
    private readonly IEntityAccessRuleSetMapper _entityAccessRuleSetMapper;

    public PageDirectoryRouteMapper(
        IEntityAccessRuleSetMapper entityAccessRuleSetMapper
        )
    {
        _entityAccessRuleSetMapper = entityAccessRuleSetMapper;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<PageDirectoryRoute> Map(IReadOnlyCollection<PageDirectory> dbPageDirectories)
    {
        const string ROOT_PATH = "/";

        ArgumentNullException.ThrowIfNull(dbPageDirectories);

        var dbRoot = dbPageDirectories.SingleOrDefault(r => !r.ParentPageDirectoryId.HasValue);
        EntityNotFoundException.ThrowIfNull(dbRoot, "ROOT");

        var root = MapRoute(dbRoot);
        var activePageDirectoryRoutes = SetChildRoutes(root, dbPageDirectories).ToList();

        root.FullUrlPath = ROOT_PATH;

        return activePageDirectoryRoutes;
    }

    private PageDirectoryRoute MapRoute(PageDirectory dbDirectory)
    {
        MissingIncludeException.ThrowIfNull(dbDirectory, d => d.PageDirectoryLocales);
        MissingIncludeException.ThrowIfNull(dbDirectory, d => d.AccessRules);

        var route = new PageDirectoryRoute
        {
            Name = dbDirectory.Name,
            PageDirectoryId = dbDirectory.PageDirectoryId,
            ParentPageDirectoryId = dbDirectory.ParentPageDirectoryId,
            UrlPath = dbDirectory.UrlPath,
            LocaleVariations = dbDirectory
                .PageDirectoryLocales
                .Select(d => new PageDirectoryRouteLocale()
                {
                    LocaleId = d.LocaleId,
                    UrlPath = d.UrlPath
                })
                .ToArray()
        };

        var accessRuleSet = _entityAccessRuleSetMapper.Map(dbDirectory);
        if (accessRuleSet != null)
        {
            route.AccessRuleSets = [accessRuleSet];
        }

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
            var mappedRoute = MapRoute(dbRoute);
            mappedRoute.FullUrlPath = CombineUrl(parent.FullUrlPath, mappedRoute.UrlPath);
            ExpandDirectoryLocales(parent, mappedRoute);
            ExpandAccessRules(parent, mappedRoute);

            foreach (var childRoute in SetChildRoutes(mappedRoute, allDbRoutes))
            {
                yield return childRoute;
            }

            yield return mappedRoute;
        }
    }

    private static void ExpandAccessRules(PageDirectoryRoute parent, PageDirectoryRoute routingInfo)
    {
        if (parent.AccessRuleSets.Count > 0)
        {
            routingInfo.AccessRuleSets = routingInfo
                .AccessRuleSets
                .Concat(parent.AccessRuleSets)
                .ToArray();
        }
    }

    /// <summary>
    /// If a locale is defined in a parent directory but not in the child we define one 
    /// using the parent info. This is so we have all locale permutations of this directory even if 
    /// locales are defined further down the directory tree.
    /// </summary>
    private static void ExpandDirectoryLocales(PageDirectoryRoute parent, PageDirectoryRoute routingInfo)
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

    private static string CombineUrl(string path1, string path2)
    {
        return string.Join("/", path1, path2);
    }
}
