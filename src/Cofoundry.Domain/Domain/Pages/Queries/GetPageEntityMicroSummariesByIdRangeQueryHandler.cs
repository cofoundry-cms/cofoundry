using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetPageEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetPageEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetPageEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public GetPageEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {

        var definition = _entityDefinitionRepository.GetRequiredByCode(PageEntityDefinition.DefinitionCode);

        var results = await _dbContext
            .PagePublishStatusQueries
            .AsNoTracking()
            .FilterActive()
            .FilterByStatus(PublishStatusQuery.Latest, executionContext.ExecutionDate)
            .Where(q => query.PageIds.Contains(q.PageId))
            .Select(q => q.PageVersion)
            .Select(v => new RootEntityMicroSummary()
            {
                RootEntityId = v.PageId,
                RootEntityTitle = v.Title,
                EntityDefinitionName = definition.Name,
                EntityDefinitionCode = definition.EntityDefinitionCode
            })
            .ToDictionaryAsync(e => e.RootEntityId);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
