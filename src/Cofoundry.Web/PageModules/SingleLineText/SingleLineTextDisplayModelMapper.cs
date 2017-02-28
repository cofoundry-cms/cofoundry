using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class SingleLineTextDisplayModelMapper : IPageModuleDisplayModelMapper<SingleLineTextDataModel>
    {
        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<SingleLineTextDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
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