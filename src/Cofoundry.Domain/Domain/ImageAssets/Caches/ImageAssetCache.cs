using Cofoundry.Core.Caching;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IImageAssetCache"/>.
/// </summary>
public class ImageAssetCache : IImageAssetCache
{
    private const string IMAGE_ASSET_RENDER_DETAILS_CACHEKEY = "ImageAssetRenderDetails:";
    private readonly IObjectCache _cache;

    public ImageAssetCache(
        IObjectCacheFactory cacheFactory
        )
    {
        _cache = cacheFactory.Get("COF_ImageAssets");
    }

    /// <inheritdoc/>
    public ImageAssetRenderDetails? GetImageAssetRenderDetailsIfCached(int imageAssetId)
    {
        var cacheKey = IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId;
        var result = _cache.Get<ImageAssetRenderDetails>(cacheKey);

        return result;
    }

    /// <inheritdoc/>
    public async Task<ImageAssetRenderDetails?> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails?>> getter)
    {
        var cacheKey = IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId;
        var result = await _cache.GetOrAddAsync(cacheKey, getter);

        return result;
    }

    /// <inheritdoc/>
    public ImageAssetRenderDetails? GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails?> getter)
    {
        var cacheKey = IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId;
        var result = _cache.GetOrAdd(cacheKey, getter);

        return result;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <inheritdoc/>
    public void Clear(int imageAssetId)
    {
        _cache.Clear(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
    }
}
