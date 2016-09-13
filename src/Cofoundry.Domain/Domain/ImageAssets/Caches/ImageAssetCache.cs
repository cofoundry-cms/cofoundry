using Cofoundry.Core.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ImageAssetCache : IImageAssetCache
    {
        #region constructor

        private const string IMAGE_ASSET_RENDER_DETAILS_CACHEKEY = "ImageAssetRenderDetails:";

        private readonly IObjectCache _cache;

        public ImageAssetCache(
            IObjectCacheFactory cacheFactory
            )
        {
            _cache = cacheFactory.Get("COF_ImageAssets");
        }

        #endregion

        #region public methods

        public ImageAssetRenderDetails GetImageAssetRenderDetailsIfCached(int imageAssetId)
        {
            return _cache.Get<ImageAssetRenderDetails>(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }

        public async Task<ImageAssetRenderDetails> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails>> getter)
        {
            return await _cache.GetOrAddAsync(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }

        public ImageAssetRenderDetails GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails> getter)
        {
            return _cache.GetOrAdd(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId, getter);
        }

        public void Clear(int imageAssetId)
        {
            _cache.Clear(IMAGE_ASSET_RENDER_DETAILS_CACHEKEY + imageAssetId);
        }

        /// <summary>
        /// Clears the specified cache entry. If the key parameter is not provided, all
        /// entries in the cache namespace are removed.
        /// </summary>
        /// <param name="key">Unique key of the cache entry to update</param>
        public void Clear()
        {
            _cache.Clear();
        }

        #endregion

    }
}
