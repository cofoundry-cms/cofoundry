using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// A mapper for mapping <see cref="PageDirectoryRoute"/> projections.
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

    /// <summary>
    /// Maps a <see cref="PageDirectory"/> data from the database to a
    /// collection of <see cref="PageDirectoryRoute"/> projections.
    /// </summary>
    /// <param name="dbPageDirectories">
    /// Entity Framework query results to map. The query must include the 
    /// <see cref="PageDirectory.PageDirectoryLocales"/> and <see cref="PageDirectory.AccessRules"/> relations.
    /// </param>
    public ICollection<PageDirectoryRoute> Map(IReadOnlyCollection<PageDirectory> dbPageDirectories)
    {
        const string ROOT_PATH = "/";

        if (dbPageDirectories == null) throw new ArgumentNullException(nameof(dbPageDirectories));

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

        route.AccessRuleSets = new List<EntityAccessRuleSet>();
        var accessRuleSet = _entityAccessRuleSetMapper.Map(dbDirectory);
        if (accessRuleSet != null)
        {
            route.AccessRuleSets.Add(accessRuleSet);
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

    private void ExpandAccessRules(PageDirectoryRoute parent, PageDirectoryRoute routingInfo)
    {
        foreach (var accessRuleSet in parent.AccessRuleSets)
        {
            routingInfo.AccessRuleSets.Add(accessRuleSet);
        };
    }

    /// <summary>
    /// If a locale is defined in a parent directory but not in the child we define one 
    /// using the parent info. This is so we have all locale permutations of this directory even if 
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
