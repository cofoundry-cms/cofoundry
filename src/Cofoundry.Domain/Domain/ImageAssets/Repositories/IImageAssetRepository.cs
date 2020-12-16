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
    public interface IImageAssetRepository
    {
        #region queries

        Task<ImageAssetDetails> GetImageAssetDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null);
        Task<ImageAssetFile> GetImageAssetFileByIdQueryAsync(int imageAssetId, IExecutionContext executionContext = null);
        Task<ImageAssetRenderDetails> GetImageAssetRenderDetailsByIdAsync(int imageAssetId, IExecutionContext executionContext = null);
        Task<IDictionary<int, ImageAssetRenderDetails>> GetImageAssetRenderDetailsByIdRangeAsync(IEnumerable<int> imageAssetIds, IExecutionContext executionContext = null);
        Task<PagedQueryResult<ImageAssetSummary>> SearchImageAssetSummariesAsync(SearchImageAssetSummariesQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        Task<int> AddImageAssetAsync(AddImageAssetCommand command, IExecutionContext executionContext = null);
        Task DeleteImageAssetAsync(int imageAssetId, IExecutionContext executionContext = null);
        Task UpdateImageAssetAsync(UpdateImageAssetCommand command, IExecutionContext executionContext = null);

        #endregion
    }
}
