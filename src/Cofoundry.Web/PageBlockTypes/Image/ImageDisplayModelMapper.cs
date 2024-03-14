namespace Cofoundry.Web;

public class ImageDisplayModelMapper : IPageBlockTypeDisplayModelMapper<ImageDataModel>
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;

    public ImageDisplayModelMapper(
        IQueryExecutor queryExecutor,
        IImageAssetRouteLibrary imageAssetRouteLibrary
        )
    {
        _queryExecutor = queryExecutor;
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
    }

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

            var image = images.GetValueOrDefault(item.DataModel.ImageId);
            displayModel.Source = _imageAssetRouteLibrary.ImageAsset(image);

            result.Add(item, displayModel);
        }
    }
}