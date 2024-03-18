﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// Looks up a route for a custom entity page using either the 
/// CustomEntityId or UrlSlug property. These route objects are 
/// cached in order to make routing lookups speedy.
/// </summary>
public class GetCustomEntityRouteByPathQueryHandler
    : IQueryHandler<GetCustomEntityRouteByPathQuery, CustomEntityRoute?>
    , IIgnorePermissionCheckHandler
{
    private readonly IQueryExecutor _queryExecutor;

    public GetCustomEntityRouteByPathQueryHandler(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<CustomEntityRoute?> ExecuteAsync(GetCustomEntityRouteByPathQuery query, IExecutionContext executionContext)
    {
        var routes = await _queryExecutor.ExecuteAsync(new GetCustomEntityRoutesByDefinitionCodeQuery(query.CustomEntityDefinitionCode), executionContext);

        var filter = routes.Where(r =>
            (r.Locale == null && !query.LocaleId.HasValue)
            || (r.Locale != null && query.LocaleId.HasValue && r.Locale.LocaleId == query.LocaleId.Value));

        if (query.CustomEntityId.HasValue)
        {
            return filter.SingleOrDefault(r => r.CustomEntityId == query.CustomEntityId.Value);
        }
        else
        {
            return filter.SingleOrDefault(r => r.UrlSlug.Equals(query.UrlSlug, StringComparison.OrdinalIgnoreCase));
        }
    }
}