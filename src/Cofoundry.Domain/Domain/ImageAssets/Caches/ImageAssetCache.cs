using Cofoundry.Core.Caching;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
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

        public ImageAssetRenderDetails GetImageAssetRenderDetailsIfCached(int imageAssetId)
        {
            return _cache.Get<ImageAssetRenderDetails>(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }

        public Task<ImageAssetRenderDetails> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails>> getter)
        {
            return _cache.GetOrAddAsync(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }

        public ImageAssetRenderDetails GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails> getter)
        {
            return _cache.GetOrAdd(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public void Clear(int imageAssetId)
        {
            _cache.Clear(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }
    }
}
