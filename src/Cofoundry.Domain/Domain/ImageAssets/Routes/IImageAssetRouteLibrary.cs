using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for image assets
    /// </summary>
    public interface IImageAssetRouteLibrary
    {
        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for</param>
        /// <param name="settings">Optional resizing settings for the image</param>
        Task<string> ImageAssetAsync(int? imageAssetId, IImageResizeSettings settings = null);

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for</param>
        /// <param name="width">width to resize the image to</param>
        /// <param name="height">height to resize the image to</param>
        Task<string> ImageAssetAsync(int? imageAssetId, int? width, int? height = null);

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        /// <param name="settings">Optional resizing settings for the image</param>
        string ImageAsset(IImageAssetRenderable asset, IImageResizeSettings settings = null);

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        /// <param name="width">width to resize the image to</param>
        /// <param name="height">height to resize the image to</param>
        string ImageAsset(IImageAssetRenderable asset, int? width, int? height = null);
    }
}
