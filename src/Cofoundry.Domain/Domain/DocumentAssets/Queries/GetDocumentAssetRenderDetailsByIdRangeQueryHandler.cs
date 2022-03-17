using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetRenderDetailsByIdRangeQueryHandler
    : IQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IDictionary<int, DocumentAssetRenderDetails>>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IDictionary<int, DocumentAssetRenderDetails>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDocumentAssetRenderDetailsMapper _documentAssetRenderDetailsMapper;

    public GetDocumentAssetRenderDetailsByIdRangeQueryHandler(
        CofoundryDbContext dbContext,
        IDocumentAssetRenderDetailsMapper documentAssetRenderDetailsMapper
        )
    {
        _dbContext = dbContext;
        _documentAssetRenderDetailsMapper = documentAssetRenderDetailsMapper;
    }

    public async Task<IDictionary<int, DocumentAssetRenderDetails>> ExecuteAsync(GetDocumentAssetRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResults = await QueryDb(query).ToListAsync();

        var mappedResults = dbResults
            .Select(_documentAssetRenderDetailsMapper.Map)
            .ToDictionary(d => d.DocumentAssetId);

        return mappedResults;
    }

    private IQueryable<DocumentAsset> QueryDb(GetDocumentAssetRenderDetailsByIdRangeQuery query)
    {
        return _dbContext
            .DocumentAssets
            .AsNoTracking()
            .FilterByIds(query.DocumentAssetIds);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetRenderDetailsByIdRangeQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}