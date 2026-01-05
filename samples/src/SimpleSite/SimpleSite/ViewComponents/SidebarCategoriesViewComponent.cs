using Microsoft.AspNetCore.Mvc;

namespace SimpleSite;

public class SidebarCategoriesViewComponent : ViewComponent
{
    private readonly IContentRepository _contentRepository;
    private readonly IVisualEditorStateService _visualEditorStateService;

    public SidebarCategoriesViewComponent(
        IContentRepository contentRepository,
        IVisualEditorStateService visualEditorStateService)
    {
        _contentRepository = contentRepository;
        _visualEditorStateService = visualEditorStateService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // We can use the current visual editor state (e.g. edit mode, live mode) to
        // determine whether to show unpublished categories in the list.
        var visualEditorState = await _visualEditorStateService.GetCurrentAsync();

        var query = new SearchCustomEntityRenderSummariesQuery()
        {
            CustomEntityDefinitionCode = CategoryCustomEntityDefinition.DefinitionCode,
            PageSize = 20,
            PublishStatus = visualEditorState.GetAmbientEntityPublishStatusQuery()
        };

        var pagedCategories = await _contentRepository
            .CustomEntities()
            .Search()
            .AsRenderSummaries(query)
            .MapItem(MapCategory)
            .ExecuteAsync();

        return View(pagedCategories.Items);
    }

    private CategorySummary MapCategory(CustomEntityRenderSummary customEntity)
    {
        var model = (CategoryDataModel)customEntity.Model;

        var category = new CategorySummary()
        {
            CategoryId = customEntity.CustomEntityId,
            Title = customEntity.Title,
            ShortDescription = model.ShortDescription
        };

        return category;
    }
}
