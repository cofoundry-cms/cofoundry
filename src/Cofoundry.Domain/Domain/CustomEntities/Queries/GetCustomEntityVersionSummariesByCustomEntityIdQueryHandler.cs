using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Gets a set of custom entity versions for a specific 
/// custom entity, ordered by create date, and returns
/// them as a paged collection of CustomEntityVersionSummary
/// projections.
/// </summary>
public class GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler
    : IQueryHandler<GetCustomEntityVersionSummariesByCustomEntityIdQuery, PagedQueryResult<CustomEntityVersionSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ICustomEntityVersionSummaryMapper _customEntityVersionSummaryMapper;

    public GetCustomEntityVersionSummariesByCustomEntityIdQueryHandler(
        CofoundryDbContext dbContext,
        IPermissionValidationService permissionValidationService,
        ICustomEntityVersionSummaryMapper customEntityVersionSummaryMapper
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
        _customEntityVersionSummaryMapper = customEntityVersionSummaryMapper;
    }

    public async Task<PagedQueryResult<CustomEntityVersionSummary>> ExecuteAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query, IExecutionContext executionContext)
    {
        var definitionCode = await _dbContext
            .CustomEntities
            .AsNoTracking()
            .Where(c => c.CustomEntityId == query.CustomEntityId)
            .Select(c => c.CustomEntityDefinitionCode)
            .FirstOrDefaultAsync();

        if (definitionCode == null)
        {
            return PagedQueryResult<CustomEntityVersionSummary>.Empty(query);
        }

        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCode, executionContext.UserContext);

        var dbVersions = await QueryVersionsAsync(query);
        var versions = _customEntityVersionSummaryMapper.MapVersions(query.CustomEntityId, dbVersions);

        return versions;
    }

    private async Task<PagedQueryResult<CustomEntityVersion>> QueryVersionsAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
    {
        return await _dbContext
            .CustomEntityVersions
            .AsNoTracking()
            .Include(e => e.Creator)
            .FilterActive()
            .FilterByCustomEntityId(query.CustomEntityId)
            .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
            .ThenByDescending(v => v.CreateDate)
            .ToPagedResultAsync(query);
    }
}