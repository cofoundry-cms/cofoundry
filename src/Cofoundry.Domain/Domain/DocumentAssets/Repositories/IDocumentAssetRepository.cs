using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over document asset data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    public interface IDocumentAssetRepository
    {
        #region queries

        Task<DocumentAssetDetails> GetDocumentAssetDetailsByIdAsync(int documentAssetId, IExecutionContext executionContext = null);
        Task<DocumentAssetFile> GetDocumentAssetFileByIdQueryAsync(int documentAssetId, IExecutionContext executionContext = null);
        Task<DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdAsync(int documentAssetId, IExecutionContext executionContext = null);
        Task<IDictionary<int, DocumentAssetRenderDetails>> GetDocumentAssetRenderDetailsByIdRangeAsync(IEnumerable<int> documentAssetIds, IExecutionContext executionContext = null);
        Task<PagedQueryResult<DocumentAssetSummary>> SearchDocumentAssetSummariesAsync(SearchDocumentAssetSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        Task<int> AddDocumentAssetAsync(AddDocumentAssetCommand command, IExecutionContext executionContext = null);
        Task DeleteDocumentAssetAsync(int documentAssetId, IExecutionContext executionContext = null);
        Task UpdateDocumentAssetAsync(UpdateDocumentAssetCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
