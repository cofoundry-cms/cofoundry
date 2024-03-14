using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class RawHtmlDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RawHtmlDataModel>
{
    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<RawHtmlDataModel> context,
        PageBlockTypeDisplayModelMapperResult<RawHtmlDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var html = string.IsNullOrEmpty(item.DataModel.RawHtml)
                ? null
                : new HtmlString(item.DataModel.RawHtml);

            var displayModel = new RawHtmlDisplayModel
            {
                RawHtml = html
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
