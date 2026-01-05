using Microsoft.AspNetCore.Mvc;

namespace Dev.Sandbox;

public class SidebarCategoriesViewComponent : ViewComponent
{
    private readonly IContentRepository _contentRepository;

    public SidebarCategoriesViewComponent(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var entities = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(new SearchCustomEntityRenderSummariesQuery()
            {
                CustomEntityDefinitionCode = CategoryCustomEntityDefinition.DefinitionCode,
                PageSize = 20,
                SortBy = CustomEntityQuerySortType.Title,
                PublishStatus = PublishStatusQuery.Published
            })
            .ExecuteAsync();

        var viewModel = MapCategories(entities);

        return View(viewModel);
    }

    private static IReadOnlyCollection<CategorySummary> MapCategories(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
    {
        var categories = new List<CategorySummary>(customEntityResult.Items.Count);

        foreach (var customEntity in customEntityResult.Items)
        {
            var model = (CategoryDataModel)customEntity.Model;

            var category = new CategorySummary
            {
                CategoryId = customEntity.CustomEntityId,
                Title = customEntity.Title,
                ShortDescription = model.ShortDescription
            };

            categories.Add(category);
        }

        return categories;
    }
}
