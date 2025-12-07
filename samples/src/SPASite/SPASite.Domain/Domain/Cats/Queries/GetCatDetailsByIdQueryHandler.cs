using SPASite.Data;

namespace SPASite.Domain;

public class GetCatDetailsByIdQueryHandler
    : IQueryHandler<GetCatDetailsByIdQuery, CatDetails?>
    , IIgnorePermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;
    private readonly SPASiteDbContext _dbContext;

    public GetCatDetailsByIdQueryHandler(
        IContentRepository contentRepository,
        SPASiteDbContext dbContext
        )
    {
        _contentRepository = contentRepository;
        _dbContext = dbContext;
    }

    public async Task<CatDetails?> ExecuteAsync(GetCatDetailsByIdQuery query, IExecutionContext executionContext)
    {
        var cat = await _contentRepository
            .CustomEntities()
            .GetById(query.CatId)
            .AsRenderSummary()
            .MapWhenNotNull(MapCatAsync)
            .ExecuteAsync();

        return cat;
    }

    private async Task<CatDetails?> MapCatAsync(CustomEntityRenderSummary customEntity)
    {
        if (customEntity.Model is not CatDataModel model)
        {
            return null;
        }

        var cat = new CatDetails
        {
            CatId = customEntity.CustomEntityId,
            Name = customEntity.Title,
            Description = model.Description,
            Breed = await GetBreedAsync(model.BreedId),
            Features = await GetFeaturesAsync(model.FeatureIds),
            Images = await GetImagesAsync(model.ImageAssetIds),
            TotalLikes = await GetLikeCount(customEntity.CustomEntityId)
        };

        return cat;
    }

    private Task<int> GetLikeCount(int catId)
    {
        return _dbContext
            .CatLikeCounts
            .AsNoTracking()
            .Where(c => c.CatCustomEntityId == catId)
            .Select(c => c.TotalLikes)
            .FirstOrDefaultAsync();
    }

    private async Task<Breed?> GetBreedAsync(int? breedId)
    {
        if (!breedId.HasValue)
        {
            return null;
        }

        var query = new GetBreedByIdQuery(breedId.Value);

        return await _contentRepository.ExecuteQueryAsync(query);
    }

    private async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync(IReadOnlyCollection<int> featureIds)
    {
        if (EnumerableHelper.IsNullOrEmpty(featureIds))
        {
            return Array.Empty<Feature>();
        }

        var query = new GetFeaturesByIdRangeQuery(featureIds);

        var features = await _contentRepository.ExecuteQueryAsync(query);

        return features
            .Select(f => f.Value)
            .OrderBy(f => f.Title)
            .ToArray();
    }

    private async Task<IReadOnlyCollection<ImageAssetRenderDetails>> GetImagesAsync(IReadOnlyCollection<int> imageAssetIds)
    {
        if (EnumerableHelper.IsNullOrEmpty(imageAssetIds))
        {
            return Array.Empty<ImageAssetRenderDetails>();
        }

        var images = await _contentRepository
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .FilterAndOrderByKeys(imageAssetIds)
            .ExecuteAsync();

        return images;
    }
}
