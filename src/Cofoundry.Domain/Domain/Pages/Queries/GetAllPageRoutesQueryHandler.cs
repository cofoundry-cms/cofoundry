namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns a collection of page routing data for all pages. The 
/// PageRoute projection is a small page object focused on providing 
/// routing data only. Data returned from this query is cached by 
/// default as it's core to routing and often incorporated in more detailed
/// page projections.
/// </summary>
public class GetAllPageRoutesQueryHandler
    : IQueryHandler<GetAllPageRoutesQuery, IReadOnlyCollection<PageRoute>>
    , IPermissionRestrictedQueryHandler<GetAllPageRoutesQuery, IReadOnlyCollection<PageRoute>>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetAllPageRoutesQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<IReadOnlyCollection<PageRoute>> ExecuteAsync(GetAllPageRoutesQuery query, IExecutionContext executionContext)
    {
        var result = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery());
        return result.Values.ToArray();
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageRoutesQuery query)
    {
        yield return new PageReadPermission();
    }
}
