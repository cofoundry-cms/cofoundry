using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RichTextDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RichTextDataModel>
    {
        public Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<RichTextDataModel> context,
            PageBlockTypeDisplayModelMapperResult<RichTextDataModel> result
            )
        {
            foreach (var item in context.Items)
            {
                var displayModel = new RichTextDisplayModel();
                displayModel.RawHtml = new HtmlString(item.DataModel.RawHtml);

                result.Add(item, displayModel);
            }

            return Task.CompletedTask;
        }
    }
}