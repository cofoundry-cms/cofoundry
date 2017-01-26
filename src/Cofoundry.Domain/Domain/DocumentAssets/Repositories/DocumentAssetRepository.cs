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
            return _queryExecutor.GetByIdAsync<DocumentAssetDetails>(documentAssetId, executionContext);
        }

        public DocumentAssetFile GetDocumentAssetFileByIdQuery(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<DocumentAssetFile>(id, executionContext);
        }

        public DocumentAssetRenderDetails GetDocumentAssetRenderDetailsById(int imageAssetId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<DocumentAssetRenderDetails>(imageAssetId, executionContext);
        }

        public Task<DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<DocumentAssetRenderDetails>(imageAssetId, executionContext);
        }

        public IDictionary<int, DocumentAssetRenderDetails> GetDocumentAssetRenderDetailsByIdRange(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRange<DocumentAssetRenderDetails>(imageAssetIds, executionContext);
        }

        public Task<IDictionary<int, DocumentAssetRenderDetails>> GetDocumentAssetRenderDetailsByIdRangeAsync(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRangeAsync<DocumentAssetRenderDetails>(imageAssetIds, executionContext);
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
