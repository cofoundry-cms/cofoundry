using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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
        /// potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to display the docment in the browser using the 
        /// "inline" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for</param>
        public async Task<string> DocumentAssetAsync(int? documentAssetId)
        {
            if (!documentAssetId.HasValue) return string.Empty;

            var asset = await GetDocumentAssetAsync(documentAssetId);

            return DocumentAsset(asset);
        }

        /// <summary>
        /// Gets the url for a document asset that displays the docment in the 
        /// browser using the "inline" content disposition.
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        public string DocumentAsset(IDocumentAssetRenderable asset)
        {
            if (asset == null) return string.Empty;

            var pathName = MakeFolderName(asset);
            var filename = MakeFileName(asset);
            var url = $"/assets/documents/{pathName}/{filename}";

            return url;
        }

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to 
        /// save a potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to download using the "attachment" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for</param>
        public async Task<string> DocumentAssetDownloadAsync(int? documentAssetId)
        {
            if (!documentAssetId.HasValue) return string.Empty;

            var asset = await GetDocumentAssetAsync(documentAssetId);

            return DocumentAssetDownload(asset);
        }

        /// <summary>
        /// Gets the url for a document asset that is set to download using
        /// the "attachment" content disposition.
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        public string DocumentAssetDownload(IDocumentAssetRenderable asset)
        {
            if (asset == null) return string.Empty;

            var pathName = MakeFolderName(asset);
            var filename = MakeFileName(asset);
            var url = $"/assets/documents/download/{pathName}/{filename}";

            return url;
        }

        #endregion

        private async Task<DocumentAssetRenderDetails> GetDocumentAssetAsync(int? documentAssetId)
        {
            var getAssetQuery = new GetDocumentAssetRenderDetailsByIdQuery(documentAssetId.Value);
            var asset = await _queryExecutor.ExecuteAsync(getAssetQuery);

            return asset;
        }

        private static string MakeFolderName(IDocumentAssetRenderable asset)
        {
            return asset.DocumentAssetId + "-" + asset.FileStamp + "-" + asset.VerificationToken;
        }

        private static string MakeFileName(IDocumentAssetRenderable asset)
        {
            return Path.ChangeExtension(SlugFormatter.ToSlug(asset.FileName), asset.FileExtension);
        }
    }
}
