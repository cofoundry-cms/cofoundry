using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetImageAssetDetailsByIdQueryHandler
    : IQueryHandler<GetImageAssetDetailsByIdQuery, ImageAssetDetails>
    , IPermissionRestrictedQueryHandler<GetImageAssetDetailsByIdQuery, ImageAssetDetails>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IImageAssetDetailsMapper _imageAssetDetailsMapper;

    public GetImageAssetDetailsByIdQueryHandler(
        CofoundryDbContext dbContext,
        IImageAssetDetailsMapper imageAssetDetailsMapper
        )
    {
        _dbContext = dbContext;
        _imageAssetDetailsMapper = imageAssetDetailsMapper;
    }

    public async Task<ImageAssetDetails> ExecuteAsync(GetImageAssetDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var dbResult = await _dbContext
            .ImageAssets
            .AsNoTracking()
            .Include(i => i.Creator)
            .Include(i => i.Updater)
            .Include(i => i.ImageAssetTags)
            .ThenInclude(i => i.Tag)
            .FilterById(query.ImageAssetId)
            .SingleOrDefaultAsync();

        var result = _imageAssetDetailsMapper.Map(dbResult);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetDetailsByIdQuery query)
    {
        yield return new ImageAssetReadPermission();
    }
}