namespace Cofoundry.Domain.Internal;

/// <summary>
/// Finds routing information for a custom entitiy by it's id. Although
/// in a typical website you wouldn't have multiple details pages for a custom entity
/// type, it is supported and the query returns a collection of routes.
/// </summary>
public class GetPageRoutingInfoByCustomEntityIdQueryHandler
    : IQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, IReadOnlyCollection<PageRoutingInfo>>
    , IPermissionRestrictedQueryHandler<GetPageRoutingInfoByCustomEntityIdQuery, IReadOnlyCollection<PageRoutingInfo>>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageRoutingInfoByCustomEntityIdQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<IReadOnlyCollection<PageRoutingInfo>> ExecuteAsync(GetPageRoutingInfoByCustomEntityIdQuery query, IExecutionContext executionContext)
    {
        var result = await _queryExecutor.ExecuteAsync(new GetPageRoutingInfoByCustomEntityIdRangeQuery([query.CustomEntityId]), executionContext);

        if (!result.ContainsKey(query.CustomEntityId))
        {
            return Array.Empty<PageRoutingInfo>();
        }

        return result[query.CustomEntityId]
            .OrderBy(e => e.PageRoute.UrlPath.Length)
            .ToArray();
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutingInfoByCustomEntityIdQuery query)
    {
        yield return new PageReadPermission();
    }
}
