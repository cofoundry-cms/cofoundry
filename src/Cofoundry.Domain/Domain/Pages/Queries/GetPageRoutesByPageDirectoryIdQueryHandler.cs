﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns page routing data for pages that are nested immediately inside the specified 
/// directory. The PageRoute projection is a small page object focused on providing 
/// routing data only. Data returned from this query is cached by 
/// default as it's core to routing and often incorporated in more detailed
/// page projections.
/// </summary>
public class GetPageRoutesByPageDirectoryIdQueryHandler
    : IQueryHandler<GetPageRoutesByPageDirectoryIdQuery, IReadOnlyCollection<PageRoute>>
    , IPermissionRestrictedQueryHandler<GetPageRoutesByPageDirectoryIdQuery, IReadOnlyCollection<PageRoute>>
{
    private readonly IQueryExecutor _queryExecutor;

    public GetPageRoutesByPageDirectoryIdQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<IReadOnlyCollection<PageRoute>> ExecuteAsync(GetPageRoutesByPageDirectoryIdQuery query, IExecutionContext executionContext)
    {
        var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
        var result = allPageRoutes.Where(p => p.PageDirectory.PageDirectoryId == query.PageDirectoryId);

        return result.ToArray();
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByPageDirectoryIdQuery query)
    {
        yield return new PageReadPermission();
    }
}