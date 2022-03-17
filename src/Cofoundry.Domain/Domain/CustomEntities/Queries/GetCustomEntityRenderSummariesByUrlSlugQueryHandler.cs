using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Returns custom entities filtered on the url slug value. This query
/// can return multiple custom entities because unique url slugs are an
/// optional setting on the custom entity definition.
/// </summary>
public class GetCustomEntityRenderSummariesByUrlSlugQueryHandler
    : IQueryHandler<GetCustomEntityRenderSummariesByUrlSlugQuery, ICollection<CustomEntityRenderSummary>>
    , IPermissionRestrictedQueryHandler<GetCustomEntityRenderSummariesByUrlSlugQuery, ICollection<CustomEntityRenderSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly ICustomEntityRenderSummaryMapper _customEntityRenderSummaryMapper;
    private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

    public GetCustomEntityRenderSummariesByUrlSlugQueryHandler(
        CofoundryDbContext dbContext,
        IQueryExecutor queryExecutor,
        ICustomEntityRenderSummaryMapper customEntityRenderSummaryMapper,
        ICustomEntityDefinitionRepository customEntityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _customEntityRenderSummaryMapper = customEntityRenderSummaryMapper;
        _customEntityDefinitionRepository = customEntityDefinitionRepository;
    }

    public async Task<ICollection<CustomEntityRenderSummary>> ExecuteAsync(GetCustomEntityRenderSummariesByUrlSlugQuery query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .CustomEntityPublishStatusQueries
            .AsNoTracking()
            .Include(e => e.CustomEntityVersion)
            .ThenInclude(e => e.CustomEntity)
            .FilterActive()
            .FilterByCustomEntityDefinitionCode(query.CustomEntityDefinitionCode)
            .FilterByCustomEntityUrlSlug(query.UrlSlug)
            .FilterByStatus(query.PublishStatus, executionContext.ExecutionDate)
            .Select(e => e.CustomEntityVersion)
            .ToListAsync();

        if (!dbResult.Any()) return Array.Empty<CustomEntityRenderSummary>();

        var result = await _customEntityRenderSummaryMapper.MapAsync(dbResult, executionContext);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetCustomEntityRenderSummariesByUrlSlugQuery query)
    {
        var definition = _customEntityDefinitionRepository.GetRequiredByCode(query.CustomEntityDefinitionCode);
        yield return new CustomEntityReadPermission(definition);
    }
}
