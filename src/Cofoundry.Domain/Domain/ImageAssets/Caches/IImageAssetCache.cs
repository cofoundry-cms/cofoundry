using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Cache for image asset data, which is frequently accessed when
    /// rendering pages and images.
    /// </summary>
    public interface IImageAssetCache
    {
        /// <summary>
        /// Gets an image if it's already cached, otherwise returns <see langword="null"/>.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return.</param>
        ImageAssetRenderDetails GetImageAssetRenderDetailsIfCached(int imageAssetId);

        /// <summary>
        /// Gets an image if it's already cached, otherwise the <paramref name="getter"/> is invoked
        /// and the result is cached and returned.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return.</param>
        /// <param name="getter">Function to invoke if the image isn't in the cache.</param>
        Task<ImageAssetRenderDetails> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails>> getter);

        /// <summary>
        /// Gets an image if it's already cached, otherwise the <paramref name="getter"/> is invoked
        /// and the result is cached and returned.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to return.</param>
        /// <param name="getter">Function to invoke if the image isn't in the cache.</param>
        ImageAssetRenderDetails GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails> getter);

        /// <summary>
        /// Clears all items in the image cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the cache entry for the specified image asset.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to clear cache entries for.</param>
        void Clear(int imageAssetId);
    }
}
