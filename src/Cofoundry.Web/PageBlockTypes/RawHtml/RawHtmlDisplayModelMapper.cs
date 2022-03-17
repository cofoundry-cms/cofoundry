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
            var displayModel = new RawHtmlDisplayModel();
            displayModel.RawHtml = new HtmlString(item.DataModel.RawHtml);

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
