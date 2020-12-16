using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to ImageAssetFile objects.
    /// </summary>
    public interface IImageAssetFileMapper
    {
        /// <summary>
        /// Maps a ImageAssetRenderDetails (which is potentially cached) into an
        /// ImageAssetFile object. If the dbImage is null then null is returned.
        /// </summary>
        /// <param name="dbImage">ImageAssetRenderDetails record to map.</param>
        /// <param name="contentStream">Steam containing the file data.</param>
        ImageAssetFile Map(ImageAssetRenderDetails dbImage, Stream contentStream);
    }
}
