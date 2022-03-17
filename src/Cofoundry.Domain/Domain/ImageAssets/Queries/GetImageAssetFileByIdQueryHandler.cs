using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetImageAssetFileByIdQueryHandler
    : IQueryHandler<GetImageAssetFileByIdQuery, ImageAssetFile>
    , IPermissionRestrictedQueryHandler<GetImageAssetFileByIdQuery, ImageAssetFile>
{
    private readonly IFileStoreService _fileStoreService;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IImageAssetFileMapper _imageAssetFileMapper;

    public GetImageAssetFileByIdQueryHandler(
        IFileStoreService fileStoreService,
        IQueryExecutor queryExecutor,
        IImageAssetFileMapper imageAssetFileMapper
        )
    {
        _fileStoreService = fileStoreService;
        _queryExecutor = queryExecutor;
        _imageAssetFileMapper = imageAssetFileMapper;
    }

    public async Task<ImageAssetFile> ExecuteAsync(GetImageAssetFileByIdQuery query, IExecutionContext executionContext)
    {
        // Render details is potentially cached so we query for this rather than directly with the db
        var getImageQuery = new GetImageAssetRenderDetailsByIdQuery(query.ImageAssetId);
        var dbResult = await _queryExecutor.ExecuteAsync(getImageQuery, executionContext);

        if (dbResult == null) return null;
        var fileName = Path.ChangeExtension(dbResult.FileNameOnDisk, dbResult.FileExtension);
        var contentStream = await _fileStoreService.GetAsync(ImageAssetConstants.FileContainerName, fileName); ;

        if (contentStream == null)
        {
            throw new FileNotFoundException("Image asset file could not be found", fileName);
        }

        var result = _imageAssetFileMapper.Map(dbResult, contentStream);

        return result;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetFileByIdQuery query)
    {
        yield return new ImageAssetReadPermission();
    }
}