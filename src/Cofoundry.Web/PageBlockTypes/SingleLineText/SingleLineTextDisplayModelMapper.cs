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
            IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<SingleLineTextDataModel>> inputCollection, 
            PublishStatusQuery publishStatusQuery
            )
        {
            return Task.FromResult(Map(inputCollection));
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