using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RichTextWithMediaDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RichTextWithMediaDataModel>
    {
        public Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(
            IEnumerable<PageBlockTypeDisplayModelMapperInput<RichTextWithMediaDataModel>> inputs, 
            PublishStatusQuery publishStatus
            )
        {
            var results = new List<PageBlockTypeDisplayModelMapperOutput>();

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