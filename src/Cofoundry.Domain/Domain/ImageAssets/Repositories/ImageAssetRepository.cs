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
    public class ImageAssetRepository : IImageAssetRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public ImageAssetRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        public Task<ImageAssetDetails> GetImageAssetDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetImageAssetDetailsByIdQuery(imageAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<ImageAssetFile> GetImageAssetFileByIdQueryAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetImageAssetFileByIdQuery(imageAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<ImageAssetRenderDetails> GetImageAssetRenderDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            var query = new GetImageAssetRenderDetailsByIdQuery(imageAssetId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<IDictionary<int, ImageAssetRenderDetails>> GetImageAssetRenderDetailsByIdRangeAsync(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null)
        {
            var query = new GetImageAssetRenderDetailsByIdRangeQuery(imageAssetIds);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PagedQueryResult<ImageAssetSummary>> SearchImageAssetSummariesAsync(SearchImageAssetSummariesQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion

        #region commands

        public async Task<int> AddImageAssetAsync(AddImageAssetCommand command, IExecutionContext executionContext = null)
        {
            await _commandExecutor.ExecuteAsync(command, executionContext);

            return command.OutputImageAssetId;
        }

        public Task DeleteImageAssetAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            var command = new DeleteImageAssetCommand() { ImageAssetId = imageAssetId };
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        public Task UpdateImageAssetAsync(UpdateImageAssetCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        #endregion
    }
}
