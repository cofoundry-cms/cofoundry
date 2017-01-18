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
        DocumentAssetFile GetDocumentAssetFileByIdQuery(int id, IExecutionContext executionContext = null);
        DocumentAssetRenderDetails GetDocumentAssetRenderDetailsById(int imageAssetId, IExecutionContext executionContext = null);
        Task<DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null);
        IDictionary<int, DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdRange(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null);
        Task<IDictionary<int, DocumentAssetRenderDetails>> GetDocumentAssetRenderDetailsByIdRangeAsync(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null);
        Task<PagedQueryResult<DocumentAssetSummary>> SearchDocumentAssetSummariesAsync(SearchDocumentAssetSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        Task<int> AddDocumentAssetAsync(AddDocumentAssetCommand command, IExecutionContext executionContext = null);
        Task DeleteDocumentAssetAsync(int imageAssetId, IExecutionContext executionContext = null);
        Task UpdateDocumentAssetAsync(UpdateDocumentAssetCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
