using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetDocumentAssetEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public GetDocumentAssetEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetDocumentAssetEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(DocumentAssetEntityDefinition.DefinitionCode);
        var results = await _dbContext
            .DocumentAssets
            .AsNoTracking()
            .FilterByIds(query.DocumentAssetIds)
            .Select(a => new RootEntityMicroSummary()
            {
                RootEntityId = a.DocumentAssetId,
                RootEntityTitle = a.Title,
                EntityDefinitionCode = definition.EntityDefinitionCode,
                EntityDefinitionName = definition.Name
            })
            .ToDictionaryAsync(e => e.RootEntityId);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}
