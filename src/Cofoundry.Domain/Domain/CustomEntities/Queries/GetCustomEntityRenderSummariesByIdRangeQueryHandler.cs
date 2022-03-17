using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Query to get a range of custom entities by a set of ids, projected as a 
/// CustomEntityRenderSummary, which is a general-purpose projection with version 
/// specific data, including a deserialized data model. The results are 
/// version-sensitive and defaults to returning published versions only, but
/// this behavior can be controlled by the publishStatus query property.
/// </summary>
public class GetCustomEntityRenderSummariesByIdRangeQueryHandler
    : IQueryHandler<GetCustomEntityRenderSummariesByIdRangeQuery, IDictionary<int, CustomEntityRenderSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
    private readonly IPermissionValidationService _permissionValidationService;

    public GetCustomEntityRenderSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
        IPermissionValidationService permissionValidationService
        )
    {
        _dbContext = dbContext;
        _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
        _permissionValidationService = permissionValidationService;
    }

    public async Task<IDictionary<int, CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResults = await QueryAsync(query, executionContext);

        EnforcePermissions(dbResults, executionContext);
        var results = await _customEntityRenderSummaryMapper.MapAsync(dbResults, executionContext);

        return results.ToDictionary(r => r.CustomEntityId);
    }

    private async Task<List<CustomEntityVersion>> QueryAsync(GetCustomEntityRenderSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        if (query.PublishStatus == PublishStatusQuery.SpecificVersion)
        {
            throw new InvalidOperationException("PublishStatusQuery.SpecificVersion not supported in GetCustomEntityRenderSummariesByDefinitionCodeQuery");
        }

        var dbQuery = await _dbContext
            .CustomEntityPublishStatusQueries
            .AsNoTracking()
            .Include(e => e.CustomEntityVersion)
            .ThenInclude(e => e.CustomEntity)
            .FilterActive()
            .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
            .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
            .ToListAsync();

        return dbQuery
            .Select(v => v.CustomEntityVersion)
            .ToList();
    }

    private void EnforcePermissions(List<CustomEntityVersion> dbResults, IExecutionContext executionContext)
    {
        var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);
    }
}