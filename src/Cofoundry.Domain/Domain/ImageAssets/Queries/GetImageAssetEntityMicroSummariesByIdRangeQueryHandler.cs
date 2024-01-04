using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetImageAssetEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IReadOnlyDictionary<int, RootEntityMicroSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IEntityDefinitionRepository _entityDefinitionRepository;

    public GetImageAssetEntityMicroSummariesByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IEntityDefinitionRepository entityDefinitionRepository
        )
    {
        _dbContext = dbContext;
        _entityDefinitionRepository = entityDefinitionRepository;
    }

    public async Task<IReadOnlyDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetImageAssetEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(ImageAssetEntityDefinition.DefinitionCode);
        var results = await _dbContext
            .ImageAssets
            .AsNoTracking()
            .FilterByIds(query.ImageAssetIds)
            .Select(v => new RootEntityMicroSummary()
            {
                RootEntityId = v.ImageAssetId,
                RootEntityTitle = v.FileName,
                EntityDefinitionCode = definition.EntityDefinitionCode,
                EntityDefinitionName = definition.Name
            }).ToDictionaryAsync(e => e.RootEntityId);

        return results;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new ImageAssetReadPermission();
    }
}
