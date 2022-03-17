namespace Cofoundry.Domain.Internal;

public class GetPageRoutesByIdRangeQueryHandler
    : IQueryHandler<GetPageRoutesByIdRangeQuery, IDictionary<int, PageRoute>>
    , IPermissionRestrictedQueryHandler<GetPageRoutesByIdRangeQuery, IDictionary<int, PageRoute>>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageRoutesByIdRangeQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<IDictionary<int, PageRoute>> ExecuteAsync(GetPageRoutesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery(), executionContext);
        var result = allPageRoutes
            .FilterByKeys(query.PageIds)
            .ToDictionary(r => r.PageId);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}