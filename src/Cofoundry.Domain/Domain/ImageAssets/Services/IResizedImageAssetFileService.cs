using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Service for retrieving image asset files and resizing them to specific 
    /// dimensions. There is no implementation of this in the base Cofoundry package 
    /// and is designed to be overriden by an image resizing library that suits your
    /// platform and scalability requirements.
    /// </summary>
    public interface IResizedImageAssetFileService
    {
        /// <summary>
        /// Resized an image asset using a set of settings like width, height and cropping type.
        /// </summary>
        /// <param name="asset">The image asset to resize.</param>
        /// <param name="settings">The settings used to resize the image.</param>
        Task<Stream> GetAsync(IImageAssetRenderable asset, IImageResizeSettings settings);

        /// <summary>
        /// Clears any cached resized images for a specific image asset.
        /// </summary>
        /// <param name="imageAssetId">The file name (without extension) of the image asset to clear the cache for.</param>
        Task ClearAsync(string fileNameOnDisk);
    }
}
