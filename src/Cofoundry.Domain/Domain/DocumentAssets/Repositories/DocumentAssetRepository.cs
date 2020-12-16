using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over image asset data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    [Obsolete("Use the new IContentRepository instead.")]
    public class DocumentAssetRepository : IDocumentAssetRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public DocumentAssetRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        public Task<DocumentAssetDetails> GetDocumentAssetDetailsByIdAsync(int documentAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetDocumentAssetDetailsByIdQuery(documentAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<DocumentAssetFile> GetDocumentAssetFileByIdQueryAsync(int documentAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetDocumentAssetFileByIdQuery(documentAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdAsync(int documentAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetDocumentAssetRenderDetailsByIdQuery(documentAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<IDictionary<int, DocumentAssetRenderDetails>> GetDocumentAssetRenderDetailsByIdRangeAsync(IEnumerable<int> documentAssetIds, IExecutionContext executionContext = null)
        {
            var query = new GetDocumentAssetRenderDetailsByIdRangeQuery(documentAssetIds);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PagedQueryResult<DocumentAssetSummary>> SearchDocumentAssetSummariesAsync(SearchDocumentAssetSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region commands

        public async Task<int> AddDocumentAssetAsync(AddDocumentAssetCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputDocumentAssetId;
        }

        public Task DeleteDocumentAssetAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            var command = new DeleteDocumentAssetCommand() { DocumentAssetId = imageAssetId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateDocumentAssetAsync(UpdateDocumentAssetCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
