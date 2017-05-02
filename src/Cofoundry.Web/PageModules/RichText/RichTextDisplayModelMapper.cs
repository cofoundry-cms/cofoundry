using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web
{
    public class RichTextDisplayModelMapper : IPageModuleDisplayModelMapper<RichTextDataModel>
    {
        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<RichTextDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            foreach (var input in inputs)
            {
                var output = new RichTextDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                yield return input.CreateOutput(output);
            }
        }
    }
}