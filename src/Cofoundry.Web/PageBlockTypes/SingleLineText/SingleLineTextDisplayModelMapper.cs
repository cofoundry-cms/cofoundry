using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class SingleLineTextDisplayModelMapper : IPageBlockDisplayModelMapper<SingleLineTextDataModel>
    {
        public Task<IEnumerable<PageBlockDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockDisplayModelMapperInput<SingleLineTextDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            return Task.FromResult(Map(inputs));
        }

        private IEnumerable<PageBlockDisplayModelMapperOutput> Map(IEnumerable<PageBlockDisplayModelMapperInput<SingleLineTextDataModel>> inputs)
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