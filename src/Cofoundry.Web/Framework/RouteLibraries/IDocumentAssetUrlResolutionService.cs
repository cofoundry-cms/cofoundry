using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper service for resolving an asset urls
    /// </summary>
    public interface IDocumentAssetUrlResolutionService
    {
        #region public methods

        string GetUrl(int? id);

        #endregion
    }
}
