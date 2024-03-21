using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetPageVersionBlockEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetPageVersionBlockEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetPageVersionBlockEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public GetPageVersionBlockEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionBlockEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(PageEntityDefinition.DefinitionCode);
        var results = await _dbContext
            .PageVersionBlocks
            .AsNoTracking()
            .FilterActive()
            .Where(m => query.PageVersionBlockIds.Contains(m.PageVersionBlockId))
            .Select(m => new ChildEntityMicroSummary()
            {
                ChildEntityId = m.PageVersionBlockId,
                RootEntityId = m.PageVersion.PageId,
                RootEntityTitle = m.PageVersion.Title,
                EntityDefinitionCode = definition.EntityDefinitionCode,
                EntityDefinitionName = definition.Name,
                IsPreviousVersion = m.PageVersion.PagePublishStatusQueries.Count == 0
            })
            .ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionBlockEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
