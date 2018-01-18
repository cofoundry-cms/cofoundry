using Cofoundry.Core;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class ContentSplitSectionDisplayModelMapper : IPageBlockTypeDisplayModelMapper<ContentSplitSectionDataModel>
    {
        private readonly IImageAssetRepository _imageAssetRepository;

        public ContentSplitSectionDisplayModelMapper(
            IImageAssetRepository imageAssetRepository
            )
        {
            _imageAssetRepository = imageAssetRepository;
        }

        public async Task<IEnumerable<PageBlockTypeDisplayModelMapperOutput>> MapAsync(
            IReadOnlyCollection<PageBlockTypeDisplayModelMapperInput<ContentSplitSectionDataModel>> inputCollection, 
            PublishStatusQuery publishStatus
            )
        {
            var imageAssetIds = inputCollection.SelectDistinctModelValuesWithoutEmpty(i => i.ImageAssetId); ;
            var imageAssets = await _imageAssetRepository.GetImageAssetRenderDetailsByIdRangeAsync(imageAssetIds);

            var results = new List<PageBlockTypeDisplayModelMapperOutput>(inputCollection.Count);

            foreach (var input in inputCollection)
            {
                var output = new ContentSplitSectionDisplayModel();
                output.HtmlText = new HtmlString(input.DataModel.HtmlText);
                output.Title = input.DataModel.Title;
                output.Image = imageAssets.GetOrDefault(input.DataModel.ImageAssetId);

                results.Add(input.CreateOutput(output));
            }

            return results;
        }
    }
}