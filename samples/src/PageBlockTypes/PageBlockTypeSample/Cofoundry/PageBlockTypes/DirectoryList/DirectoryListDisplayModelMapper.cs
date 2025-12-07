namespace PageBlockTypeSample;

public class DirectoryListDisplayModelMapper : IPageBlockTypeDisplayModelMapper<DirectoryListDataModel>
{
    private readonly IContentRepository _contentRepository;

    public DirectoryListDisplayModelMapper(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<DirectoryListDataModel> context,
        PageBlockTypeDisplayModelMapperResult<DirectoryListDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var query = new SearchPageRenderSummariesQuery
            {
                PageDirectoryId = item.DataModel.PageDirectoryId,
                PageSize = item.DataModel.PageSize,
                // Pass through the workflow status so that we only show published pages 
                // when viewing the live site.
                PublishStatus = context.PublishStatusQuery
            };

            var pages = await _contentRepository
                .WithContext(context.ExecutionContext)
                .Pages()
                .Search()
                .AsRenderSummaries(query)
                .ExecuteAsync();

            var displayModel = new DirectoryListDisplayModel
            {
                Pages = pages
            };

            result.Add(item, displayModel);
        }
    }
}
