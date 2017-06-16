using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetImageAssetRenderDetailsByIdRangeQueryHandler 
        : IQueryHandler<GetByIdRangeQuery<ImageAssetRenderDetails>, IDictionary<int, ImageAssetRenderDetails>>
        , IAsyncQueryHandler<GetByIdRangeQuery<ImageAssetRenderDetails>, IDictionary<int, ImageAssetRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetByIdRangeQuery<ImageAssetRenderDetails>, IDictionary<int, ImageAssetRenderDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IImageAssetCache _imageAssetCache;

        public GetImageAssetRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IImageAssetCache imageAssetCache
            )
        {
            _dbContext = dbContext;
            _imageAssetCache = imageAssetCache;
        }

        #endregion

        #region execution

        public IDictionary<int, ImageAssetRenderDetails> Execute(GetByIdRangeQuery<ImageAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var cachedResults = QueryCache(query.Ids);
            var missingResultsQuery = QueryDb(cachedResults);
            List<ImageAssetRenderDetails> missingResults = null;

            if (missingResultsQuery != null)
            {
                missingResults = missingResultsQuery.ToList();
            }

            return AddResultsToCacheAndReturnResult(cachedResults, missingResults);
        }

        public async Task<IDictionary<int, ImageAssetRenderDetails>> ExecuteAsync(GetByIdRangeQuery<ImageAssetRenderDetails> query, IExecutionContext executionContext)
        {
            var cachedResults = QueryCache(query.Ids);
            var missingResultsQuery = QueryDb(cachedResults);
            List<ImageAssetRenderDetails> missingResults = null;

            if (missingResultsQuery != null)
            {
                missingResults = await missingResultsQuery.ToListAsync();
            }
            return AddResultsToCacheAndReturnResult(cachedResults, missingResults);
        }

        #endregion

        #region private query classes

        private class ImageCacheResult
        {
            public int ImageAssetId { get; set; }

            public ImageAssetRenderDetails Model { get; set; }
        }

        #endregion

        #region private methods

        private List<ImageCacheResult> QueryCache(int[] ids)
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

        private IQueryable<ImageAssetRenderDetails> QueryDb(List<ImageCacheResult> cacheResults)
        {
            var missingIds = cacheResults
                .Where(r => r.Model == null)
                .Select(r => r.ImageAssetId)
                .ToArray();

            if (!missingIds.Any()) return null;

            return _dbContext
                .ImageAssets
                .AsNoTracking()
                .FilterByIds(missingIds)
                .ProjectTo<ImageAssetRenderDetails>();
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdRangeQuery<ImageAssetRenderDetails> query)
        {
            yield return new ImageAssetReadPermission();
        }

        #endregion
    }
}
