using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.BasicTestSite;

public class SidebarCategoriesViewComponent : ViewComponent
{
    private readonly IContentRepository _contentRepository;

    public SidebarCategoriesViewComponent(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var query = new SearchCustomEntityRenderSummariesQuery();
        query.CustomEntityDefinitionCode = CategoryCustomEntityDefinition.DefinitionCode;
        query.PageSize = 20;
        query.SortBy = CustomEntityQuerySortType.Title;
        query.PublishStatus = PublishStatusQuery.Published;

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

    private ICollection<CategorySummary> MapCategories(PagedQueryResult<CustomEntityRenderSummary> customEntityResult)
    {
        var categories = new List<CategorySummary>(customEntityResult.Items.Count());

        foreach (var customEntity in customEntityResult.Items)
        {
            var model = (CategoryDataModel)customEntity.Model;

            var category = new CategorySummary();
            category.CategoryId = customEntity.CustomEntityId;
            category.Title = customEntity.Title;
            category.ShortDescription = model.ShortDescription;

            categories.Add(category);
        }

        return categories;
    }
}
