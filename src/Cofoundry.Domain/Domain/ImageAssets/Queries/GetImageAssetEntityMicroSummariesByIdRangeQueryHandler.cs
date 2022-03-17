using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetImageAssetEntityMicroSummariesByIdRangeQueryHandler
    : IQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
    , IPermissionRestrictedQueryHandler<GetImageAssetEntityMicroSummariesByIdRangeQuery, IDictionary<int, RootEntityMicroSummary>>
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

    public async Task<IDictionary<int, RootEntityMicroSummary>> ExecuteAsync(GetImageAssetEntityMicroSummariesByIdRangeQuery query, IExecutionContext executionContext)
    {
        var results = await Query(query).ToDictionaryAsync(e => e.RootEntityId);

        return results;
    }

    private IQueryable<RootEntityMicroSummary> Query(GetImageAssetEntityMicroSummariesByIdRangeQuery query)
    {
        var definition = _entityDefinitionRepository.GetRequiredByCode(ImageAssetEntityDefinition.DefinitionCode);

        var dbQuery = _dbContext
            .ImageAssets
            .AsNoTracking()
            .FilterByIds(query.ImageAssetIds)
            .Select(v => new RootEntityMicroSummary()
            {
                RootEntityId = v.ImageAssetId,
                RootEntityTitle = v.FileName,
                EntityDefinitionCode = definition.EntityDefinitionCode,
                EntityDefinitionName = definition.Name
            });

        return dbQuery;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetEntityMicroSummariesByIdRangeQuery query)
    {
        yield return new ImageAssetReadPermission();
    }
}
