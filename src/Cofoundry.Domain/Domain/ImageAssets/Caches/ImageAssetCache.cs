using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Cache for image asset data, which is frequently accessed when
    /// rendering pages and images.
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

        /// <summary>
        /// Gets an image if it's already cached, otherwise returns null
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return</param>
        public ImageAssetRenderDetails GetImageAssetRenderDetailsIfCached(int imageAssetId)
        {
            return _cache.Get<ImageAssetRenderDetails>(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }

        /// <summary>
        /// Gets an image if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return</param>
        /// <param name="getter">Function to invoke if the image isn't in the cache</param>
        public Task<ImageAssetRenderDetails> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails>> getter)
        {
            return _cache.GetOrAddAsync(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }
        /// <summary>
        /// Gets an image if it's already cached, otherwise the getter is invoked
        /// and the result is cached and returned
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return</param>
        /// <param name="getter">Function to invoke if the image isn't in the cache</param>
        public ImageAssetRenderDetails GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails> getter)
        {
            return _cache.GetOrAdd(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }

        /// <summary>
        /// Clears all items in the image cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to clear cache entries for.</param>
        public void Clear(int imageAssetId)
        {
            _cache.Clear(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }
    }
}
