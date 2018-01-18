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
            IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<RichTextWithMediaDataModel>> inputCollection, 
            PublishStatusQuery publishStatusQuery
            )
        {
            var results = new List<PageBlockTypeDisplayModelMapperOutput>(inputCollection.Count);

            foreach (var input in inputCollection)
            {
                var output = new RichTextWithMediaDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}