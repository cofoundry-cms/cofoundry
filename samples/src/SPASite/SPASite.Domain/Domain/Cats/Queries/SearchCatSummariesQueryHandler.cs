using SPASite.Data;

namespace SPASite.Domain;

public class SearchCatSummariesQueryHandler
    : IQueryHandler<SearchCatSummariesQuery, PagedQueryResult<CatSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly SPASiteDbContext _dbContext;
    private readonly IContentRepository _contentRepository;

    public SearchCatSummariesQueryHandler(
        SPASiteDbContext dbContext,
        IContentRepository contentRepository)
    {
        _dbContext = dbContext;
        _contentRepository = contentRepository;
    }

    public async Task<PagedQueryResult<CatSummary>> ExecuteAsync(SearchCatSummariesQuery query, IExecutionContext executionContext)
    {
        var customEntityQuery = new SearchCustomEntityRenderSummariesQuery
        {
            CustomEntityDefinitionCode = CatCustomEntityDefinition.Code,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
            PublishStatus = PublishStatusQuery.Published,
            SortBy = CustomEntityQuerySortType.PublishDate
        };

        var catCustomEntities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(customEntityQuery)
            .ExecuteAsync();

        var allMainImages = await GetMainImages(catCustomEntities);
        var allLikeCounts = await GetLikeCounts(catCustomEntities);

        return MapCats(catCustomEntities, allMainImages, allLikeCounts);
    }

    private Task<IReadOnlyDictionary<int, ImageAssetRenderDetails>> GetMainImages(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
    {
        var imageAssetIds = customEntityResult
            .Items
            .Select(i => (CatDataModel)i.Model)
            .Where(m => !EnumerableHelper.IsNullOrEmpty(m.ImageAssetIds))
            .Select(m => m.ImageAssetIds.First())
            .Distinct();

        return _contentRepository
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();
    }

    private Task<Dictionary<int, int>> GetLikeCounts(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
    {
        var catIds = customEntityResult
            .Items
            .Select(i => i.CustomEntityId)
            .Distinct()
            .ToList();

        return _dbContext
            .CatLikeCounts
            .AsNoTracking()
            .Where(c => catIds.Contains(c.CatCustomEntityId))
            .ToDictionaryAsync(c => c.CatCustomEntityId, c => c.TotalLikes);
    }

    private static PagedQueryResult<CatSummary> MapCats(
        PagedQueryResult<CustomEntityRenderSummary> customEntityResult,
        IReadOnlyDictionary<int, ImageAssetRenderDetails> images,
        IReadOnlyDictionary<int, int> allLikeCounts)
    {
        var cats = new List<CatSummary>(customEntityResult.Items.Count);

        foreach (var customEntity in customEntityResult.Items)
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

        return customEntityResult.ChangeType(cats);
    }
}
