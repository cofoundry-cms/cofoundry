using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetImageAssetRenderDetailsByIdRangeQueryHandler 
        : IQueryHandler<GetImageAssetRenderDetailsByIdRangeQuery, IDictionary<int, ImageAssetRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetImageAssetRenderDetailsByIdRangeQuery, IDictionary<int, ImageAssetRenderDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;
        private readonly IImageAssetRenderDetailsMapper _imageAssetRenderDetailsMapper;

        public GetImageAssetRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache,
            IImageAssetRenderDetailsMapper imageAssetRenderDetailsMapper
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
            _imageAssetRenderDetailsMapper = imageAssetRenderDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, ImageAssetRenderDetails>> ExecuteAsync(GetImageAssetRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            var cachedResults = QueryCache(query.ImageAssetIds);
            var missingResultsQuery = QueryDb(cachedResults);
            List<ImageAssetRenderDetails> missingResults = null;

            if (missingResultsQuery != null)
            {
                var dbMissingResults = await missingResultsQuery.ToListAsync();
                missingResults = dbMissingResults
                    .Select(_imageAssetRenderDetailsMapper.Map)
                    .ToList();
            }

            return AddResultsToCacheAndReturnResult(cachedResults, missingResults);
        }

        #region private query classes

        private class ImageCacheResult
        {
            public int ImageAssetId { get; set; }

            public ImageAssetRenderDetails Model { get; set; }
        }

        #endregion

        private List<ImageCacheResult> QueryCache(IReadOnlyCollection<int> ids)
        {
            var results = new List<ImageCacheResult>();

            foreach (var id in ids)
            {
                var result = new ImageCacheResult();
                result.ImageAssetId = id;
                result.Model = _imageAssetCache.GetImageAssetRenderDetailsIfCached(id);
                results.Add(result);
            }

            return results;
        }

        private IQueryable<ImageAsset> QueryDb(List<ImageCacheResult> cacheResults)
        {
            var missingIds = cacheResults
                .Where(r => r.Model == null)
                .Select(r => r.ImageAssetId)
                .ToArray();

            if (!missingIds.Any()) return null;

            return _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterByIds(missingIds);
        }

        private IDictionary<int, ImageAssetRenderDetails> AddResultsToCacheAndReturnResult(List<ImageCacheResult> results, List<ImageAssetRenderDetails> missingResults)
        {
            if (missingResults != null)
            {
                foreach (var missingResult in missingResults)
                {
                    // Add the result to the cache
                    var resultToAdd = _imageAssetCache.GetOrAdd(missingResult.ImageAssetId, () =>
                    {
                        return missingResult;
                    });

                    // then add it into the result collection
                    var result = results.Single(r => r.ImageAssetId == missingResult.ImageAssetId);
                    result.Model = resultToAdd;
                }
            }

            return results
                .Where(r => r.Model != null)
                .Select(r => r.Model)
                .ToDictionary(i => i.ImageAssetId);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetRenderDetailsByIdRangeQuery query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
