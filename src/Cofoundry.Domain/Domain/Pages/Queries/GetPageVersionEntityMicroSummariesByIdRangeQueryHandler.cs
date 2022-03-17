using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetPageVersionEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetPageVersionEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
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

    public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetPageVersionEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var results = await Query(query).ToDictionaryAsync(e => e.ChildEntityId, e => (RootEntityMicroSummary)e);

        return results;
    }

    private IQueryable<ChildEntityMicroSummary> Query(GetPageVersionEntityMicroSummariesByIdRangeQuery query)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(PageEntityDefinition.DefinitionCode);

        var dbQuery = _dbContext
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
                IsPreviousVersion = !v.PagePublishStatusQueries.Any() // not draft or latest published version
            });

        return dbQuery;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageVersionEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new PageReadPermission();
    }
}
