using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RichTextWithMediaDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RichTextWithMediaDataModel>
{
    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<RichTextWithMediaDataModel> context,
        PageBlockTypeDisplayModelMapperResult<RichTextWithMediaDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var html = string.IsNullOrEmpty(item.DataModel.RawHtml)
                ? null
                : new HtmlString(item.DataModel.RawHtml);

            var displayModel = new RichTextWithMediaDisplayModel
            {
                RawHtml = html
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
