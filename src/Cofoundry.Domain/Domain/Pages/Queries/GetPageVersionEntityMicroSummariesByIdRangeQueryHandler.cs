using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetPageVersionEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public GetPageVersionEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(PageEntityDefinition.DefinitionCode);
        var results = await _dbContext
            .PageVersions
            .AsNoTracking()
            .Where(v => query.PageVersionIds.Contains(v.PageVersionId))
            .Select(v => new ChildEntityMicroSummary()
            {
                ChildEntityId = v.PageVersionId,
                RootEntityId = v.PageId,
                RootEntityTitle = v.Title,
                EntityDefinitionCode = definition.EntityDefinitionCode,
                EntityDefinitionName = definition.Name,
                IsPreviousVersion = v.PagePublishStatusQueries.Count == 0 // not draft or latest published version
            })
            .ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
