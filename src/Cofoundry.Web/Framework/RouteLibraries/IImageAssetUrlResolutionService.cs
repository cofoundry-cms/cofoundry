using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper service for resolving an asset urls
    /// </summary>
    public interface IImageAssetUrlResolutionService
    {
        #region public methods

        string GetUrl(int? id, IImageResizeSettings settings = null);
        
        string GetDataUrl(int? id, IImageResizeSettings settings = null);
        string GetDataUrl(IImageAssetRenderable asset, IImageResizeSettings settings = null);

        #endregion
    }
}
