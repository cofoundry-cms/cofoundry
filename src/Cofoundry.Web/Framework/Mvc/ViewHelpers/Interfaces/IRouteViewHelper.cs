using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public interface IRouteViewHelper
    {
        #region pages

        string Page(IPageRoute route);
        string Page(IPageRoute route, SiteViewerMode siteViewerMode);
        string Page(IPageRoute route, int versionId);

        #endregion

        #region images

        string ImageAsset(int id, IImageResizeSettings settings = null);
        string ImageAsset(IImageAssetRenderable image, IImageResizeSettings settings = null);
        string ImageAsset(IImageAssetRenderable image, int? width, int? height = null);
        string GetDataUrl(int? id, IImageResizeSettings settings = null);
        string GetDataUrl(IImageAssetRenderable asset, IImageResizeSettings settings = null);

        #endregion
    }
}
