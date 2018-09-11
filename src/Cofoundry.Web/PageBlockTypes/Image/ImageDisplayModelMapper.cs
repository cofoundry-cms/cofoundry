using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class ImageDisplayModelMapper : IPageBlockTypeDisplayModelMapper<ImageDataModel>
    {
        #region Constructor

        private IQueryExecutor _queryExecutor;
        private IImageAssetRouteLibrary _imageAssetRouteLibrary;

        public ImageDisplayModelMapper(
            IQueryExecutor queryExecutor,
            IImageAssetRouteLibrary imageAssetRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _imageAssetRouteLibrary = imageAssetRouteLibrary;
        }

        #endregion

        public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<ImageDataModel> context, 
            PageBlockTypeDisplayModelMapperResult<ImageDataModel> result
            )
        {
            var imageAssetIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.ImageId);
            var imagesQuery = new GetImageAssetRenderDetailsByIdRangeQuery(imageAssetIds);
            var images = await _queryExecutor.ExecuteAsync(imagesQuery, context.ExecutionContext);

            foreach (var item in context.Items)
            {
                var displayModel = new ImageDisplayModel()
                {
                    AltText = item.DataModel.AltText,
                    LinkPath = item.DataModel.LinkPath,
                    LinkTarget = item.DataModel.LinkTarget
                };

                var image = images.GetOrDefault(item.DataModel.ImageId);
                displayModel.Source = _imageAssetRouteLibrary.ImageAsset(image);

                result.Add(item, displayModel);
            }
        }
    }
}