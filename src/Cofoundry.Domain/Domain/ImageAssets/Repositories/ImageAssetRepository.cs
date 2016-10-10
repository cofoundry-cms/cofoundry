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
    public class ImageAssetRepository
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
            return _queryExecutor.GetByIdAsync<ImageAssetDetails>(imageAssetId, executionContext);
        }

        public ImageAssetFile GetImageAssetFileByIdQuery(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<ImageAssetFile>(id, executionContext);
        }

        public ImageAssetRenderDetails GetImageAssetRenderDetailsById(int imageAssetId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<ImageAssetRenderDetails>(imageAssetId, executionContext);
        }

        public Task<ImageAssetRenderDetails> GetImageAssetRenderDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<ImageAssetRenderDetails>(imageAssetId, executionContext);
        }

        public IDictionary<int, ImageAssetRenderDetails> GetImageAssetRenderDetailsByIdRange(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRange<ImageAssetRenderDetails>(imageAssetIds, executionContext);
        }

        public Task<IDictionary<int, ImageAssetRenderDetails>> GetImageAssetRenderDetailsByIdRangeAsync(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdRangeAsync<ImageAssetRenderDetails>(imageAssetIds, executionContext);
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
