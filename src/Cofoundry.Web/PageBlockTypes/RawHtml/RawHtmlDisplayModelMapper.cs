using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class RawHtmlDisplayModelMapper : IPageBlockTypeDisplayModelMapper<RawHtmlDataModel>
    {
        public Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(
            IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<RawHtmlDataModel>> inputCollection, 
            PublishStatusQuery publishStatusQuery
            )
        {
            var results = new List<PageBlockTypeDisplayModelMapperOutput>(inputCollection.Count);

            foreach (var input in inputCollection)
            {
                var output = new RawHtmlDisplayModel();
                output.RawHtml = new HtmlString(input.DataModel.RawHtml);

                results.Add(input.CreateOutput(output));
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}