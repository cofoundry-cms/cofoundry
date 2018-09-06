using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for all the core Cofoundry routes.
    /// </summary>
    public interface IContentRouteLibrary
    {
        #region pages

        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        Task<string> PageAsync(int? pageId);

        /// <summary>
        /// Gets the full url of a page.
        /// </summary>
        string Page(IPageRoute route);

        /// <summary>
        /// Gets the full (relative) url of a custom entity details page.
        /// </summary>
        string Page(ICustomEntityRoutable customEntity);

        #endregion

        #region images

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for.</param>
        /// <param name="settings">Optional resizing settings for the image.</param>
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
        /// <param name="image">Image asset to get the url for.</param>
        /// <param name="settings">Optional resizing settings for the image.</param>
        string ImageAsset(IImageAssetRenderable image, IImageResizeSettings settings = null);

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="image">Image asset to get the url for.</param>
        /// <param name="width">width to resize the image to.</param>
        /// <param name="height">height to resize the image to.</param>
        string ImageAsset(IImageAssetRenderable image, int? width, int? height = null);

        #endregion

        #region documents

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to save a 
        /// potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to display the docment in the browser using the 
        /// "inline" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for.</param>
        Task<string> DocumentAssetAsync(int? documentAssetId);

        /// <summary>
        /// Gets the url for a document asset that displays the docment in the 
        /// browser using the "inline" content disposition.
        /// </summary>
        /// <param name="asset">asset to get the url for.</param>
        string DocumentAsset(IDocumentAssetRenderable asset);

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to 
        /// save a potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to download using the "attachment" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for.</param>
        Task<string> DocumentAssetDownloadAsync(int? documentAssetId);

        /// <summary>
        /// Gets the url for a document asset that is set to download using
        /// the "attachment" content disposition.
        /// </summary>
        /// <param name="asset">Document asset to get the url for.</param>
        string DocumentAssetDownload(IDocumentAssetRenderable asset);

        #endregion

        #region utility

        /// <summary>
        /// Maps a relative url to an absolute one.
        /// </summary>
        /// <param name="url">The relative url to map.</param>
        string ToAbsolute(string url);

        #endregion
    }
}
