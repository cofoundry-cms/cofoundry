using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetFile objects.
    /// </summary>
    public interface IImageAssetFileMapper
    {
        /// <summary>
        /// Maps a ImageAssetRenderDetails (which is potentially cached) into an
        /// ImageAssetFile object. If the cachedImage is null then null is returned.
        /// </summary>
        /// <param name="cachedImage">ImageAssetRenderDetails record from the cache.</param>
        /// <param name="contentStream">Steam containing the file data.</param>
        ImageAssetFile Map(ImageAssetRenderDetails dbImage, Stream contentStream);
    }
}
