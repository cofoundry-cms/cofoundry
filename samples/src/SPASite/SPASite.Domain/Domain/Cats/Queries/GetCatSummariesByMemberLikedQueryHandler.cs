using SPASite.Data;

namespace SPASite.Domain;

public class GetCatSummariesByMemberLikedQueryHandler
    : IQueryHandler<GetCatSummariesByMemberLikedQuery, IReadOnlyCollection<CatSummary>>
    , ISignedInPermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;
    private readonly SPASiteDbContext _dbContext;

    public GetCatSummariesByMemberLikedQueryHandler(
        IContentRepository contentRepository,
        SPASiteDbContext dbContext
        )
    {
        _contentRepository = contentRepository;
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CatSummary>> ExecuteAsync(GetCatSummariesByMemberLikedQuery query, IExecutionContext executionContext)
    {
        var userCatIds = await _dbContext
            .CatLikes
            .AsNoTracking()
            .Where(c => c.UserId == query.UserId)
            .OrderByDescending(c => c.CreateDate)
            .Select(c => c.CatCustomEntityId)
            .ToListAsync();

        // GetByIdRange queries return a dictionary to make lookups easier, so we 
        // have an extra step to do if we want to set the ordering to match the
        // original id collection.
        var catCustomEntities = await _contentRepository
            .CustomEntities()
            .GetByIdRange(userCatIds)
            .AsRenderSummaries()
            .FilterAndOrderByKeys(userCatIds)
            .ExecuteAsync();

        var allMainImages = await GetMainImages(catCustomEntities);
        var allLikeCounts = await GetLikeCounts(catCustomEntities);

        return MapCats(catCustomEntities, allMainImages, allLikeCounts);
    }

    private async Task<IReadOnlyDictionary<int, ImageAssetRenderDetails>> GetMainImages(IReadOnlyCollection<CustomEntityRenderSummary> customEntities)
    {
        var imageAssetIds = customEntities
            .Select(i => (CatDataModel)i.Model)
            .Where(m => !EnumerableHelper.IsNullOrEmpty(m.ImageAssetIds))
            .Select(m => m.ImageAssetIds.First())
            .Distinct();

        var result = await _contentRepository
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        return result;
    }

    private Task<Dictionary<int, int>> GetLikeCounts(IReadOnlyCollection<CustomEntityRenderSummary> customEntities)
    {
        var catIds = customEntities
            .Select(i => i.CustomEntityId)
            .Distinct()
            .ToArray();

        return _dbContext
            .CatLikeCounts
            .AsNoTracking()
            .Where(c => catIds.Contains(c.CatCustomEntityId))
            .ToDictionaryAsync(c => c.CatCustomEntityId, c => c.TotalLikes);
    }

    private static List<CatSummary> MapCats(
        IReadOnlyCollection<CustomEntityRenderSummary> customEntities,
        IReadOnlyDictionary<int, ImageAssetRenderDetails> images,
        IReadOnlyDictionary<int, int> allLikeCounts
        )
    {
        var cats = new List<CatSummary>(customEntities.Count);

        foreach (var customEntity in customEntities)
        {
            var model = (CatDataModel)customEntity.Model;

            var cat = new CatSummary
            {
                CatId = customEntity.CustomEntityId,
                Name = customEntity.Title,
                Description = model.Description,
                TotalLikes = allLikeCounts.GetValueOrDefault(customEntity.CustomEntityId)
            };

            if (!EnumerableHelper.IsNullOrEmpty(model.ImageAssetIds))
            {
                cat.MainImage = images.GetValueOrDefault(model.ImageAssetIds.FirstOrDefault());
            }

            cats.Add(cat);
        }

        return cats;
    }
}
