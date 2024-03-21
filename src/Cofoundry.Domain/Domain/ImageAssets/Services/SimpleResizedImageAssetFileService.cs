namespace Cofoundry.Domain.Internal;

/// <summary>
/// Service for resizing and caching the resulting image.
/// </summary>
public class SimpleResizedImageAssetFileService : IResizedImageAssetFileService
{
    private readonly IQueryExecutor _queryExecutor;

    public SimpleResizedImageAssetFileService(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    public Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings settings)
    {
        // Resizing only supported via plugin
        return GetFileStreamAsync(asset.ImageAssetId);
    }

    public Task ClearAsync(string fileNameOnDisk)
    {
        // nothing to clear
        return Task.CompletedTask;
    }

    private async Task<Stream> GetFileStreamAsync(int imageAssetId)
    {
        var getImageQuery = new GetImageAssetFileByIdQuery(imageAssetId);
        var result = await _queryExecutor.ExecuteAsync(getImageQuery);

        if (result == null || result.ContentStream == null)
        {
            throw new FileNotFoundException(imageAssetId.ToString(CultureInfo.InvariantCulture));
        }

        return result.ContentStream;
    }
}
