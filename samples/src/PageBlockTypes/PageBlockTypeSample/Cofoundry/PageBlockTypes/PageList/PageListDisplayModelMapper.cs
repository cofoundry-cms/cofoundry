using Cofoundry.Core;

namespace PageBlockTypeSample;

public class PageListDisplayModelMapper : IPageBlockTypeDisplayModelMapper<PageListDataModel>
{
    private readonly IContentRepository _contentRepository;

    public PageListDisplayModelMapper(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<PageListDataModel> context,
        PageBlockTypeDisplayModelMapperResult<PageListDataModel> result)
    {
        var allPageIds = context.Items.SelectManyDistinctModelValues(m => m.PageIds);

        // Page routes are cached and so are the quickest way to get simple page information.
        // If we needed more data we could use a different but slower query to get it.
        var allPageRoutes = await _contentRepository
            .WithContext(context.ExecutionContext)
            .Pages()
            .GetByIdRange(allPageIds)
            .AsRoutes()
            .ExecuteAsync();

        foreach (var item in context.Items)
        {
            // Here will get the relevant pages and order them correctly. 
            // Additionally if we are viewing the published version of the page
            // then we make sure we only show published pages in the list.
            var pages = allPageRoutes
                .FilterAndOrderByKeys(item.DataModel.PageIds)
                .Where(p => context.PublishStatusQuery != PublishStatusQuery.Published || p.IsPublished())
                .ToArray();

            var displayModel = new PageListDisplayModel()
            {
                Pages = pages
            };

            result.Add(item, displayModel);
        }
    }
}
