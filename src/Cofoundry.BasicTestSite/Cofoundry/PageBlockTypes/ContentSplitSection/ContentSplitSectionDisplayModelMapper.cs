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
        private readonly IContentRepository _contentRepository;

        public ContentSplitSectionDisplayModelMapper(
            IContentRepository contentRepository
            )
        {
            _contentRepository = contentRepository;
        }

        public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<ContentSplitSectionDataModel> context,
            PageBlockTypeDisplayModelMapperResult<ContentSplitSectionDataModel> result
            )
        {
            var imageAssetIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.ImageAssetId);
            var imageAssets = await _contentRepository
                .WithExecutionContext(context.ExecutionContext)
                .ImageAssets()
                .GetByIdRange(imageAssetIds)
                .AsRenderDetails()
                .ExecuteAsync();

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