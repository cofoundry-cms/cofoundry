using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetRenderDetailsByIdQueryHandler
    : IQueryHandler<GetDocumentAssetRenderDetailsByIdQuery, DocumentAssetRenderDetails>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetRenderDetailsByIdQuery, DocumentAssetRenderDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDocumentAssetRenderDetailsMapper _documentAssetRenderDetailsMapper;

    public GetDocumentAssetRenderDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IDocumentAssetRenderDetailsMapper documentAssetRenderDetailsMapper
        )
    {
        _dbContext = dbContext;
        _documentAssetRenderDetailsMapper = documentAssetRenderDetailsMapper;
    }

    public async Task<DocumentAssetRenderDetails> ExecuteAsync(GetDocumentAssetRenderDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await Query(query).SingleOrDefaultAsync();
        var mappedResult = _documentAssetRenderDetailsMapper.Map(dbResult);

        return mappedResult;
    }

    private IQueryable<DocumentAsset> Query(GetDocumentAssetRenderDetailsByIdQuery query)
    {
        return _dbContext
            .DocumentAssets
            .AsNoTracking()
            .FilterById(query.DocumentAssetId);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetRenderDetailsByIdQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}