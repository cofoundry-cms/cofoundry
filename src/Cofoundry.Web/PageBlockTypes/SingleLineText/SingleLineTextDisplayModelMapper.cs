using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class SingleLineTextDisplayModelMapper : IPageBlockTypeDisplayModelMapper<SingleLineTextDataModel>
{
    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<SingleLineTextDataModel> context,
        PageBlockTypeDisplayModelMapperResult<SingleLineTextDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var html = string.IsNullOrEmpty(item.DataModel.Text)
                ? null
                : new HtmlString(item.DataModel.Text);

            var displayModel = new SingleLineTextDisplayModel
            {
                Text = html
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
