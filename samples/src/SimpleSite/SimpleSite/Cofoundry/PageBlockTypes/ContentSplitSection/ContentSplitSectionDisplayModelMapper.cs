using Microsoft.AspNetCore.Html;

namespace SimpleSite;

/// <summary>
/// A IPageBlockDisplayModelMapper class handles the mapping from
/// a display model to a data model.
/// 
/// The mapper supports DI which gives you flexibility in what data
/// you want to include in the display model and how you want to 
/// map it. Mapping is done in batch to improve performance when 
/// the same block type is used multiple times on a page.
/// </summary>
public class ContentSplitSectionDisplayModelMapper : IPageBlockTypeDisplayModelMapper<ContentSplitSectionDataModel>
{
    private readonly IContentRepository _contentRepository;

    public ContentSplitSectionDisplayModelMapper(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<ContentSplitSectionDataModel> context,
        PageBlockTypeDisplayModelMapperResult<ContentSplitSectionDataModel> result
        )
    {
        // Because mapping is done in batch, we have to map multiple images here
        // The Cofoundry IContentRepository gives us an easy to use data access api.
        // for us
        var imageAssetIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.ImageAssetId);
        var imageAssets = await _contentRepository
            .WithContext(context.ExecutionContext)
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        foreach (var item in context.Items)
        {
            var displayModel = new ContentSplitSectionDisplayModel()
            {
                HtmlText = new HtmlString(item.DataModel.HtmlText),
                Title = item.DataModel.Title,
                Image = imageAssets.GetValueOrDefault(item.DataModel.ImageAssetId)
            };

            result.Add(item, displayModel);
        }
    }
}
