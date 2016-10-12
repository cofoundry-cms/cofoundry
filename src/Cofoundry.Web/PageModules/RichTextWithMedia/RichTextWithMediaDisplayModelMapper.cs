using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.Web;

namespace Cofoundry.Web
{
    public class RichTextWithMediaDisplayModelMapper : IPageModuleDisplayModelMapper<RichTextWithMediaDataModel>
    {
        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<RichTextWithMediaDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            foreach (var input in inputs)
            {
                var output = new RichTextWithMediaDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                yield return input.CreateOutput(output);
            }
        }
    }
}