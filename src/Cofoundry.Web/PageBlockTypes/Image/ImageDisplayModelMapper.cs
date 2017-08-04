using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using AutoMapper;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class ImageDisplayModelMapper : IPageBlockDisplayModelMapper<ImageDataModel>
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

        public async Task<IEnumerable<PageBlockDisplayModelMapperOutput>> MapAsync(IEnumerable<PageBlockDisplayModelMapperInput<ImageDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var images = await _queryExecutor.GetByIdRangeAsync<ImageAssetRenderDetails>(inputs.Select(i => i.DataModel.ImageId));
            var results = new List<PageBlockDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var output = Mapper.Map<ImageDisplayModel>(input.DataModel);

                var image = images.GetOrDefault(input.DataModel.ImageId);
                output.Source = _imageAssetRouteLibrary.ImageAsset(image);


                results.Add(input.CreateOutput(output));
            }

            return results.AsEnumerable();
        }
    }
}