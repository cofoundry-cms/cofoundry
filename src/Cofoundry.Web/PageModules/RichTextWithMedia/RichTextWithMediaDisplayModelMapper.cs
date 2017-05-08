using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RichTextWithMediaDisplayModelMapper : IPageModuleDisplayModelMapper<RichTextWithMediaDataModel>
    {
        public Task<IEnumerable<PageModuleDisplayModelMapperOutput>> MapAsync(IEnumerable<PageModuleDisplayModelMapperInput<RichTextWithMediaDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var results = new List<PageModuleDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = new RichTextWithMediaDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}