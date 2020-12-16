using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Route library for all the core Cofoundry routes.
    /// </summary>
    public class ContentRouteLibrary : IContentRouteLibrary
    {
        private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;
        private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;
        private readonly IPageRouteLibrary _pageRouteLibrary;
        private readonly ISiteUrlResolver _siteUriResolver;

        public ContentRouteLibrary(
            IPageRouteLibrary pageRouteLibrary,
            IImageAssetRouteLibrary imageAssetRouteLibrary,
            IDocumentAssetRouteLibrary documentAssetRouteLibrary,
            ISiteUrlResolver siteUriResolver
            )
        {
            _pageRouteLibrary = pageRouteLibrary;
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
            _documentAssetRouteLibrary = documentAssetRouteLibrary;
            _siteUriResolver = siteUriResolver;
        }

        #region pages

        /// <summary>
        /// Simple but less efficient way of getting a page url if you only know 
        /// the id. Use the overload accepting an IPageRoute if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        public Task<string> PageAsync(int? pageId)
        {
            return _pageRouteLibrary.PageAsync(pageId);
        }

        /// <summary>
        /// Gets the full url of a page.
        /// </summary>
        public string Page(IPageRoute route)
        {
            return _pageRouteLibrary.Page(route);
        }

        /// <summary>
        /// Gets the full (relative) url of a custom entity details page.
        /// </summary>
        public string Page(ICustomEntityRoutable customEntity)
        {
            return _pageRouteLibrary.Page(customEntity);
        }

        #endregion

        #region images

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for.</param>
        /// <param name="settings">Optional resizing settings for the image.</param>
        public Task<string> ImageAssetAsync(int? imageAssetId, IImageResizeSettings settings = null)
        {
            return _imageAssetRouteLibrary.ImageAssetAsync(imageAssetId, settings);
        }

        /// <summary>
        /// Simple but less efficient way of getting an image url if you only know 
        /// the id. Use the overload accepting an IImageAssetRenderable if possible to save a 
        /// potential db query if the route isn't cached.
        /// </summary>
        /// <param name="imageAssetId">Id of the image asset to get the url for</param>
        /// <param name="width">width to resize the image to</param>
        /// <param name="height">height to resize the image to</param>
        public Task<string> ImageAssetAsync(int? imageAssetId, int? width, int? height = null)
        {
            return _imageAssetRouteLibrary.ImageAssetAsync(imageAssetId, width, height);
        }

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="image">Image asset to get the url for.</param>
        /// <param name="settings">Optional resizing settings for the image.</param>
        public string ImageAsset(IImageAssetRenderable image, IImageResizeSettings settings = null)
        {
            return _imageAssetRouteLibrary.ImageAsset(image, settings);
        }

        /// <summary>
        /// Gets the url for an image asset, with optional resizing parameters
        /// </summary>
        /// <param name="image">Image asset to get the url for.</param>
        /// <param name="width">width to resize the image to.</param>
        /// <param name="height">height to resize the image to.</param>
        public string ImageAsset(IImageAssetRenderable image, int? width, int? height = null)
        {
            return _imageAssetRouteLibrary.ImageAsset(image, width, height);
        }

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
        public Task<string> DocumentAssetAsync(int? documentAssetId)
        {
            return _documentAssetRouteLibrary.DocumentAssetAsync(documentAssetId);
        }

        /// <summary>
        /// Gets the url for a document asset that displays the docment in the 
        /// browser using the "inline" content disposition.
        /// </summary>
        /// <param name="asset">asset to get the url for.</param>
        public string DocumentAsset(IDocumentAssetRenderable asset)
        {
            return _documentAssetRouteLibrary.DocumentAsset(asset);
        }

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to 
        /// save a potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to download using the "attachment" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for.</param>
        public Task<string> DocumentAssetDownloadAsync(int? documentAssetId)
        {
            return _documentAssetRouteLibrary.DocumentAssetDownloadAsync(documentAssetId);
        }

        /// <summary>
        /// Gets the url for a document asset that is set to download using
        /// the "attachment" content disposition.
        /// </summary>
        /// <param name="asset">Document asset to get the url for.</param>
        public string DocumentAssetDownload(IDocumentAssetRenderable asset)
        {
            return _documentAssetRouteLibrary.DocumentAssetDownload(asset);
        }

        #endregion

        #region utility

        /// <summary>
        /// Maps a relative url to an absolute one.
        /// </summary>
        /// <param name="url">The relative url to map.</param>
        public string ToAbsolute(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;

            return _siteUriResolver.MakeAbsolute(url);
        }

        #endregion
    }
}
