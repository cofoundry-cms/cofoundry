namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns a page directory with the specified id as a PageDirectoryRoute instance.
/// </summary>
public class GetPageDirectoryRouteByIdQueryHandler
    : IQueryHandler<GetPageDirectoryRouteByIdQuery, PageDirectoryRoute>
    , IPermissionRestrictedQueryHandler<GetPageDirectoryRouteByIdQuery, PageDirectoryRoute>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageDirectoryRouteByIdQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<PageDirectoryRoute> ExecuteAsync(GetPageDirectoryRouteByIdQuery query, IExecutionContext executionContext)
    {
        var allRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageDirectoryRoutesQuery(), executionContext);
        var result = allRoutes.SingleOrDefault(l => l.PageDirectoryId == query.PageDirectoryId);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryRouteByIdQuery command)
    {
        yield return new PageDirectoryReadPermission();
    }
}
