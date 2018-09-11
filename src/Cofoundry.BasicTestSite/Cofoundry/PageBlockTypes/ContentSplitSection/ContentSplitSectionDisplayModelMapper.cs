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

        public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<ContentSplitSectionDataModel> context,
            PageBlockTypeDisplayModelMapperResult<ContentSplitSectionDataModel> result
            )
        {
            var imageAssetIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.ImageAssetId);
            var imageAssets = await _imageAssetRepository.GetImageAssetRenderDetailsByIdRangeAsync(imageAssetIds, context.ExecutionContext);

            foreach (var item in context.Items)
            {
                var displayModel = new ContentSplitSectionDisplayModel();
                displayModel.HtmlText = new HtmlString(item.DataModel.HtmlText);
                displayModel.Title = item.DataModel.Title;
                displayModel.Image = imageAssets.GetOrDefault(item.DataModel.ImageAssetId);

                result.Add(item, displayModel);
            }
        }
    }
}