using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetDocumentAssetDetailsByIdQueryHandler
    : IQueryHandler<GetDocumentAssetDetailsByIdQuery, DocumentAssetDetails>
    , IPermissionRestrictedQueryHandler<GetDocumentAssetDetailsByIdQuery, DocumentAssetDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDocumentAssetDetailsMapper _documentAssetDetailsMapper;

    public GetDocumentAssetDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IDocumentAssetDetailsMapper documentAssetDetailsMapper
        )
    {
        _dbContext = dbContext;
        _documentAssetDetailsMapper = documentAssetDetailsMapper;
    }

    public async Task<DocumentAssetDetails> ExecuteAsync(GetDocumentAssetDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .DocumentAssets
            .AsNoTracking()
            .Include(a => a.Creator)
            .Include(a => a.Updater)
            .Include(a => a.DocumentAssetTags)
            .ThenInclude(a => a.Tag)
            .FilterById(query.DocumentAssetId)
            .SingleOrDefaultAsync();

        var result = _documentAssetDetailsMapper.Map(dbResult);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetDetailsByIdQuery query)
    {
        yield return new DocumentAssetReadPermission();
    }
}