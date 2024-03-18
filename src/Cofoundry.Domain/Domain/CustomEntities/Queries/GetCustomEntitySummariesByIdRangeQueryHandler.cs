﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// An id range query for custom entities which returns basic
/// custom entity information with workflow status and model data for the
/// latest version. The query is not version-sensitive and is designed to be 
/// used in the admin panel and not in a version-sensitive context such as a 
/// public webpage.
/// </summary>
public class GetCustomEntitySummariesByIdRangeQueryHandler
    : IQueryHandler<GetCustomEntitySummariesByIdRangeQuery, IReadOnlyDictionary<int, CustomEntitySummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly ICustomEntitySummaryMapper _customEntitySummaryMapper;

    public GetCustomEntitySummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        IPermissionValidationService permissionValidationService,
        ICustomEntitySummaryMapper customEntitySummaryMapper
        )
    {
        _dbContext = dbContext;
        _permissionValidationService = permissionValidationService;
        _customEntitySummaryMapper = customEntitySummaryMapper;
    }

    public async Task<IReadOnlyDictionary<int, CustomEntitySummary>> ExecuteAsync(GetCustomEntitySummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResults = await QueryAsync(query, executionContext);

        // Validation permissions
        var definitionCodes = dbResults.Select(r => r.CustomEntity.CustomEntityDefinitionCode);
        _permissionValidationService.EnforceCustomEntityPermission<CustomEntityReadPermission>(definitionCodes, executionContext.UserContext);

        var mappedResults = await _customEntitySummaryMapper.MapAsync(dbResults, executionContext);

        return mappedResults.ToImmutableDictionary(r => r.CustomEntityId);
    }

    private async Task<IReadOnlyCollection<CustomEntityPublishStatusQuery>> QueryAsync(GetCustomEntitySummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResults = await _dbContext
            .CustomEntityPublishStatusQueries
            .AsNoTracking()
            .Include(e => e.CustomEntityVersion)
            .ThenInclude(e => e.Creator)
            .Include(e => e.CustomEntity)
            .ThenInclude(e => e.Creator)
            .Where(v => query.CustomEntityIds.Contains(v.CustomEntityId))
            .FilterActive()
            .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
            .ToArrayAsync();

        return dbResults;
    }
}