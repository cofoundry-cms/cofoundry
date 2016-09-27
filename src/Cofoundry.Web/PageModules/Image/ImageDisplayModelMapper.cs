using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using AutoMapper;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    public class ImageDisplayModelMapper : IPageModuleDisplayModelMapper<ImageDataModel>
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

        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<ImageDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var images = _queryExecutor.GetByIdRange<ImageAssetRenderDetails>(inputs.Select(i => i.DataModel.ImageId));

            foreach (var input in inputs)
            {
                var output = Mapper.Map<ImageDisplayModel>(input.DataModel);

                var image = images.GetOrDefault(input.DataModel.ImageId);
                output.Source = _imageAssetRouteLibrary.ImageAsset(image);
                
                yield return input.CreateOutput(output);
            }
        }
    }
}