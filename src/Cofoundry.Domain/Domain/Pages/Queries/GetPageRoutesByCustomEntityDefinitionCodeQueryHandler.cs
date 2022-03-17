namespace Cofoundry.Domain.Internal;

public class GetPageRoutesByCustomEntityDefinitionCodeQueryHandler
    : IQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, ICollection<PageRoute>>
    , IPermissionRestrictedQueryHandler<GetPageRoutesByCustomEntityDefinitionCodeQuery, ICollection<PageRoute>>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public GetPageRoutesByCustomEntityDefinitionCodeQueryHandler(
        IQueryExecutor queryExecutor,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _queryExecutor = queryExecutor;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    public async Task<ICollection<PageRoute>> ExecuteAsync(GetPageRoutesByCustomEntityDefinitionCodeQuery query, IExecutionContext executionContext)
    {
        var allPageRoutes = await _queryExecutor.ExecuteAsync(new GetAllPageRoutesQuery(), executionContext);
        var customEntityRoutes = allPageRoutes
            .Where(p => p.CustomEntityDefinitionCode == query.CustomEntityDefinitionCode)
            .OrderBy(p => p.Locale != null)
            .ThenBy(p => p.Title)
            .ToList();

        return customEntityRoutes;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageRoutesByCustomEntityDefinitionCodeQuery query)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(query.CustomEntityDefinitionCode);

        yield return new CustomEntityReadPermission(definition);
        yield return new PageReadPermission();
    }
}

