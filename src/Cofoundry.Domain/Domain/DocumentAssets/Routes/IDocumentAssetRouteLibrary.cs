using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Route library for document assets
    /// </summary>
    public interface IDocumentAssetRouteLibrary
    {
        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to save a 
        /// potential db query if the asset isn't cached.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for</param>
        Task<string> DocumentAssetAsync(int? documentAssetId);

        /// <summary>
        /// Gets the url for a document asset
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        string DocumentAsset(IDocumentAssetRenderable asset);

        /// <summary>
        /// Simple but less efficient way of getting a document url if you only know 
        /// the id. Use the overload accepting an IDocumentAssetRenderable if possible to 
        /// save a potential db query if the asset isn't cached. This method generates a route
        /// with a response that is set to download using the "attachment" content disposition.
        /// </summary>
        /// <param name="documentAssetId">Id of the document asset to get the url for</param>
        Task<string> DocumentAssetDownloadAsync(int? documentAssetId);

        /// <summary>
        /// Gets the url for a document asset that is set to download using
        /// the "attachment" content disposition.
        /// </summary>
        /// <param name="asset">asset to get the url for</param>
        string DocumentAssetDownload(IDocumentAssetRenderable asset);
    }
}
