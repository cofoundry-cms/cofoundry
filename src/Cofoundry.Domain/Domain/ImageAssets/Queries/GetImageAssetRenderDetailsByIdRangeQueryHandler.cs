using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class GetImageAssetRenderDetailsByIdRangeQueryHandler
    : IQueryHandler<GetImageAssetRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, ImageAssetRenderDetails>>
    , IPermissionRestrictedQueryHandler<GetImageAssetRenderDetailsByIdRangeQuery, IReadOnlyDictionary<int, ImageAssetRenderDetails>>
{
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

    public async Task<IReadOnlyDictionary<int, ImageAssetRenderDetails>> ExecuteAsync(GetImageAssetRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
    {
        var cachedResults = QueryCache(query.ImageAssetIds);
        var missingResultsQuery = QueryDb(cachedResults);
        IReadOnlyCollection<ImageAssetRenderDetails>? missingResults = null;

        if (missingResultsQuery != null)
        {
            var dbMissingResults = await missingResultsQuery.ToArrayAsync();
            missingResults = dbMissingResults
                .Select(i => _imageAssetRenderDetailsMapper.Map(i))
                .ToArray();
        }

        return AddResultsToCacheAndReturnResult(cachedResults, missingResults);
    }

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

    private IQueryable<ImageAsset>? QueryDb(List<ImageCacheResult> cacheResults)
    {
        var missingIds = cacheResults
            .Where(r => r.Model == null)
            .Select(r => r.ImageAssetId)
            .ToArray();

        if (missingIds.Length == 0)
        {
            return null;
        }

        return _dbContext
            .ImageAssets
            .AsNoTracking()
            .FilterByIds(missingIds);
    }

    private IReadOnlyDictionary<int, ImageAssetRenderDetails> AddResultsToCacheAndReturnResult(
        IReadOnlyCollection<ImageCacheResult> results,
        IReadOnlyCollection<ImageAssetRenderDetails>? missingResults
        )
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
            .Select(r => r.Model)
            .WhereNotNull()
            .ToImmutableDictionary(i => i.ImageAssetId);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetImageAssetRenderDetailsByIdRangeQuery query)
    {
        yield return new ImageAssetReadPermission();
    }

    private class ImageCacheResult
    {
        public int ImageAssetId { get; set; }

        public ImageAssetRenderDetails? Model { get; set; }
    }
}