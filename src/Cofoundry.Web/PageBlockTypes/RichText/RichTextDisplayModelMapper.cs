using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RichTextDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RichTextDataModel>
{
    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<RichTextDataModel> context,
        PageBlockTypeDisplayModelMapperResult<RichTextDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var html = string.IsNullOrEmpty(item.DataModel.RawHtml)
                ? null
                : new HtmlString(item.DataModel.RawHtml);

            var displayModel = new RichTextDisplayModel
            {
                RawHtml = html
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
