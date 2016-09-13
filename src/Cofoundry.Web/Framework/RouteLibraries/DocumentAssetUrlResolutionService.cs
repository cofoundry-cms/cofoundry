using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper service for resolving an asset urls
    /// </summary>
    public class DocumentAssetUrlResolutionService : IDocumentAssetUrlResolutionService
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public DocumentAssetUrlResolutionService(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        public string GetUrl(int? id)
        {
            if (!id.HasValue) return string.Empty;

            var asset = _queryExecutor.GetById<DocumentAssetRenderDetails>(id.Value);

            return AssetRouteLibrary.DocumentAsset(asset);
        }

        #endregion
    }
}
