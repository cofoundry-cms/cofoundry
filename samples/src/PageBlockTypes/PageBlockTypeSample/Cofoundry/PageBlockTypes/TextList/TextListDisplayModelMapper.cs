namespace Cofoundry.Web;

public class TextListDisplayModelMapper : IPageBlockTypeDisplayModelMapper<TextListDataModel>
{
    private static readonly string[] LINE_DELIMITERS = ["\r\n", "\n"];

    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<TextListDataModel> context,
        PageBlockTypeDisplayModelMapperResult<TextListDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var displayModel = new TextListDisplayModel
            {
                TextListItems = item.DataModel.TextList.Split(LINE_DELIMITERS, StringSplitOptions.None),
                Title = item.DataModel.Title,
                IsNumbered = item.DataModel.IsNumbered
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
