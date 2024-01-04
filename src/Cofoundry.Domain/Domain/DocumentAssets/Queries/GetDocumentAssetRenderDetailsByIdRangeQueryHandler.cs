using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetRenderDetailsByIdRangeQueryHandler
    : IQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, DocumentAssetRenderDetails>>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, DocumentAssetRenderDetails>>
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

    public async Task<IReadOnlyDictionary<int, DocumentAssetRenderDetails>> ExecuteAsync(GetDocumentAssetRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
    {
        var dbResults = await _dbContext
            .DocumentAssets
            .AsNoTracking()
            .FilterByIds(query.DocumentAssetIds)
            .ToArrayAsync();

        var mappedResults = dbResults
            .Select(d => _documentAssetRenderDetailsMapper.Map(d))
            .ToImmutableDictionary(d => d.DocumentAssetId);

        return mappedResults;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetRenderDetailsByIdRangeQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}