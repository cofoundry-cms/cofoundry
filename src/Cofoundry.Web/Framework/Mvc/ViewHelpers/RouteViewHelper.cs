using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RouteViewHelper : IRouteViewHelper
    {
        private readonly IImageAssetUrlResolutionService _imageAssetUrlResolutionService;

        public RouteViewHelper(
            IImageAssetUrlResolutionService imageAssetUrlResolutionService
            )
        {
            _imageAssetUrlResolutionService = imageAssetUrlResolutionService;
        }

        #region pages

        public string Page(IPageRoute route)
        {
            return Page(route, SiteViewerMode.Any);
        }

        public string Page(IPageRoute route, SiteViewerMode siteViewerMode)
        {
            if (route == null) return string.Empty;

            var url = route.FullPath;

            if (siteViewerMode != SiteViewerMode.Any)
            {
                url = url + "?mode=" + siteViewerMode.ToString().ToLowerInvariant();
            }

            return url;
        }

        public string Page(IPageRoute route, int versionId)
        {
            if (route == null) return string.Empty;

            var url = route.FullPath;
            url = url + "?version=" + versionId;

            return url;
        }

        #endregion

        #region images
        
        public string ImageAsset(int id, IImageResizeSettings settings = null)
        {
            return _imageAssetUrlResolutionService.GetUrl(id, settings);
        }

        public string ImageAsset(IImageAssetRenderable image, IImageResizeSettings settings = null)
        {
            return AssetRouteLibrary.ImageAsset(image, settings);
        }

        public string ImageAsset(IImageAssetRenderable image, int? width, int? height = null)
        {
            return AssetRouteLibrary.ImageAsset(image, width, height);
        }

        public string GetDataUrl(int? id, IImageResizeSettings settings = null)
        {
            return _imageAssetUrlResolutionService.GetDataUrl(id, settings);
        }

        public string GetDataUrl(IImageAssetRenderable asset, IImageResizeSettings settings = null)
        {
            return _imageAssetUrlResolutionService.GetDataUrl(asset, settings);
        }

        #endregion
    }
}
