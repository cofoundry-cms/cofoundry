using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IImageAssetCache
    {
        ImageAssetRenderDetails GetImageAssetRenderDetailsIfCached(int imageAssetId);
        Task<ImageAssetRenderDetails> GetOrAddAsync(int imageAssetId, Func<Task<ImageAssetRenderDetails>> getter);
        ImageAssetRenderDetails GetOrAdd(int imageAssetId, Func<ImageAssetRenderDetails> getter);

        void Clear();
        void Clear(int imageAssetId);
    }
}
