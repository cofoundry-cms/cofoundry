using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class SingleLineTextDisplayModelMapper : IPageModuleDisplayModelMapper<SingleLineTextDataModel>
    {
        public Task<IEnumerable<PageModuleDisplayModelMapperOutput>> MapAsync(IEnumerable<PageModuleDisplayModelMapperInput<SingleLineTextDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            return Task.FromResult(Map(inputs));
        }

        private IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<SingleLineTextDataModel>> inputs)
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