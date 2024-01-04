using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Finds routing information for a set of custom entities by their ids. Although
/// in a typical website you wouldn't have multiple details pages for a custom entity
/// type, it is supported and so each custom entity id in the query returns a collection
/// of routes.
/// </summary>
public class GetPageRoutingInfoByCustomEntityIdRangeQueryHandler
    : IQueryHandler<GetPageRoutingInfoByCustomEntityIdRangeQuery, IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>>>
    , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByCustomEntityIdRangeQuery, IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IQueryExecutor _queryExecutor;

    public GetPageRoutingInfoByCustomEntityIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor
        )
    {
        _dbContext = dbContext;
        _queryExecutor = queryExecutor;
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>>> ExecuteAsync(GetPageRoutingInfoByCustomEntityIdRangeQuery query, IExecutionContext executionContext)
    {
        var idSets = await GetIdSetsAsync(query);

        if (idSets.Count == 0)
        {
            return ImmutableDictionary<int, IReadOnlyCollection<PageRoutingInfo>>.Empty;
        }

        var customEntityRoutesQueries = GetCustomEntityRoutingQuery(idSets);

        var customEntityRoutes = new Dictionary<int, CustomEntityRoute>();
        foreach (var customEntityRoutesQuery in customEntityRoutesQueries)
        {
            // Probably cached so should be quick
            var routes = await _queryExecutor.ExecuteAsync(customEntityRoutesQuery, executionContext);
            foreach (var route in routes)
            {
                if (!customEntityRoutes.ContainsKey(route.CustomEntityId))
                {
                    customEntityRoutes.Add(route.CustomEntityId, route);
                }
            }
        }

        var pageRoutesQuery = new GetPageRoutesByIdRangeQuery(idSets.Select(p => p.PageId).Distinct());
        var pageRoutes = await _queryExecutor.ExecuteAsync(pageRoutesQuery, executionContext);

        return await MapAsync(executionContext, idSets, customEntityRoutes, pageRoutes);
    }

    private class IdQueryResult
    {
        public int PageId { get; set; }
        public int CustomEntityId { get; set; }
        public string CustomEntityDefinitionCode { get; set; } = string.Empty;
    }

    private async Task<IReadOnlyCollection<IdQueryResult>> GetIdSetsAsync(GetPageRoutingInfoByCustomEntityIdRangeQuery query)
    {
        var dbResult = await _dbContext
            .Pages
            .AsNoTracking()
            .FilterActive()
            .Join(_dbContext.CustomEntities, p =>
                new { p.CustomEntityDefinitionCode, p.LocaleId },
                e => new { e.CustomEntityDefinitionCode, e.LocaleId }!,
                (p, e) => new
                {
                    CustomEntity = e,
                    Page = p
                })
            .Where(r => query.CustomEntityIds.Contains(r.CustomEntity.CustomEntityId))
            .Select(r => new IdQueryResult()
            {
                PageId = r.Page.PageId,
                CustomEntityId = r.CustomEntity.CustomEntityId,
                CustomEntityDefinitionCode = r.CustomEntity.CustomEntityDefinitionCode
            })
            .ToArrayAsync();

        return dbResult;
    }

    private async Task<IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>>> MapAsync(
        IExecutionContext executionContext,
        IReadOnlyCollection<IdQueryResult> idSets,
        Dictionary<int, CustomEntityRoute> customEntityRoutes,
        IReadOnlyDictionary<int, PageRoute> pageRoutes
        )
    {
        var allRules = await _queryExecutor.ExecuteAsync(new GetAllCustomEntityRoutingRulesQuery(), executionContext);

        var result = new List<PageRoutingInfo>(idSets.Count);

        foreach (var idSet in idSets)
        {
            var routingInfo = new PageRoutingInfo();

            var pageRoute = pageRoutes.GetValueOrDefault(idSet.PageId);
            EntityNotFoundException.ThrowIfNull(pageRoute, idSet.PageId);
            routingInfo.PageRoute = pageRoute;

            routingInfo.CustomEntityRoute = customEntityRoutes.GetOrDefault(idSet.CustomEntityId);
            EntityNotFoundException.ThrowIfNull(routingInfo.CustomEntityRoute, idSet.PageId);

            routingInfo.CustomEntityRouteRule = allRules.FirstOrDefault(r => r.RouteFormat == routingInfo.PageRoute.UrlPath);

            result.Add(routingInfo);
        }

        var groupedResult = result
            .GroupBy(r => r.CustomEntityRoute!.CustomEntityId)
            .ToImmutableDictionary(g => g.Key, g => (IReadOnlyCollection<PageRoutingInfo>)g.ToArray());

        return groupedResult;
    }

    private static IEnumerable<GetCustomEntityRoutesByDefinitionCodeQuery> GetCustomEntityRoutingQuery(IReadOnlyCollection<IdQueryResult> idSets)
    {
        return idSets
            .Select(i => i.CustomEntityDefinitionCode)
            .Distinct()
            .Select(c => new GetCustomEntityRoutesByDefinitionCodeQuery(c));
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByCustomEntityIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
