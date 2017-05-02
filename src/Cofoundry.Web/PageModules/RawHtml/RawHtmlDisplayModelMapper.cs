using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web
{
    public class RawHtmlDisplayModelMapper : IPageModuleDisplayModelMapper<RawHtmlDataModel>
    {
        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<RawHtmlDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            foreach (var input in inputs)
            {
                var output = new RawHtmlDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                yield return input.CreateOutput(output);
            }
        }
    }
}