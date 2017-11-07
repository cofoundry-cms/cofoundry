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
        public Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(
            IEnumerable<PageBlockTypeDisplayModelMapperInput<SingleLineTextDataModel>> inputs, 
            PublishStatusQuery publishStatus
            )
        {
            return Task.FromResult(Map(inputs));
        }

        private IEnumerable<PageBlockTypeDisplayModelMapperOutput> Map(IEnumerable<PageBlockTypeDisplayModelMapperInput<SingleLineTextDataModel>> inputs)
        {
            foreach (var input in inputs)
            {
                var output = new SingleLineTextDisplayModel();
                output.Text = new HtmlString(input.DataModel.Text);

                yield return input.CreateOutput(output);
            }
        }
    }
}