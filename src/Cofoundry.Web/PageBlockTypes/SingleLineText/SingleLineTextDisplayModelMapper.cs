using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class SingleLineTextDisplayModelMapper : IPageBlockTypeDisplayModelMapper<SingleLineTextDataModel>
    {
        public Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<SingleLineTextDataModel> context,
            PageBlockTypeDisplayModelMapperResult<SingleLineTextDataModel> result
            )
        {
            foreach (var item in context.Items)
            {
                var displayModel = new SingleLineTextDisplayModel();
                displayModel.Text = new HtmlString(item.DataModel.Text);

                result.Add(item, displayModel);
            }

            return Task.CompletedTask;
        }
    }
}