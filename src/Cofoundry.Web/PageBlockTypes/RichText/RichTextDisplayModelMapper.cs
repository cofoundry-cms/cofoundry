using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RichTextDisplayModelMapper : IPageBlockDisplayModelMapper<RichTextDataModel>
    {
        public Task<IEnumerable<PageBlockDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockDisplayModelMapperInput<RichTextDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var results = new List<PageBlockDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = new RichTextDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}