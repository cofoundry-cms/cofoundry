using Cofoundry.Core;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web;

public class QuotationDisplayModelMapper : IPageBlockTypeDisplayModelMapper<QuotationDataModel>
{
    public Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<QuotationDataModel> context,
        PageBlockTypeDisplayModelMapperResult<QuotationDataModel> result
        )
    {
        foreach (var item in context.Items)
        {
            var displayModel = new QuotationDisplayModel
            {
                CitationText = item.DataModel.CitationText,
                CitationUrl = item.DataModel.CitationUrl,
                Quotation = new HtmlString(HtmlFormatter.ConvertLineBreaksToBrTags(item.DataModel.CitationText)),
                Title = item.DataModel.Title
            };

            result.Add(item, displayModel);
        }

        return Task.CompletedTask;
    }
}
