using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for document assets
    /// </summary>
    public class DocumentAssetRouteLibrary : IDocumentAssetRouteLibrary
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public DocumentAssetRouteLibrary(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to save a 
        /// potential db query if the asset isn't cached.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for</param>
        public async Task<string> DocumentAssetAsync(int? documentAssetId)
        {
            if (!documentAssetId.HasValue) return string.Empty;

            var asset = await _queryExecutor.GetByIdAsync<DocumentAssetRenderDetails>(documentAssetId.Value);

            return DocumentAsset(asset);
        }

        /// <summary>
        /// Gets the url for a document asset
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        public string DocumentAsset(IDocumentAssetRenderable asset)
        {
            if (asset == null) return string.Empty;

            return DocumentAsset(asset.DocumentAssetId, asset.FileName, asset.FileExtension);
        }

        #endregion

        private static string DocumentAsset(int assetId, string fileName, string extension)
        {
            var fn = Path.ChangeExtension(assetId + "_" + SlugFormatter.ToSlug(fileName), extension);
            var url = "/assets/files/" + fn;

            return url;
        }
    }
}
